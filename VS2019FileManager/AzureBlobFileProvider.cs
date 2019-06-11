using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;

namespace E.VS2019FileManager
{
	public class AzureBlobFileProvider : IFileProvider
	{
		string AzureStorageAccountName = "dxdonwebinar";
		string AzureStorageAccountKey = "hFLNIcaxIitIdGbkvOvsixKIwTdKTfIf6roY8yahwMKdIXdYpYwr87ErgbYpwuQETKsWnQQjViH6ypKZ9e4Bvg==";
		string AzureBlobStorageContainerName = "dxfilemanager";

		const string EmptyFolderDummyBlobName = "aspxAzureEmptyFolderBlob";

		CloudBlobClient _client;
		CloudBlobClient Client
		{
			get
			{
				if (this._client == null)
				{
					StorageCredentials credentials = new StorageCredentials(AzureStorageAccountName, AzureStorageAccountKey);
					CloudStorageAccount account = new CloudStorageAccount(credentials, true);
					this._client = account.CreateCloudBlobClient();
				}
				return this._client;
			}
		}

		CloudBlobContainer _container;
		CloudBlobContainer Container
		{
			get
			{
				if (this._container == null)
				{
					this._container = Client.GetContainerReference(AzureBlobStorageContainerName);
				}
				return this._container;
			}
		}

		public async Task<IList<IClientFileSystemItem>> GetDirectoryContentsAsync(string dirKey)
		{
			var result = new List<IClientFileSystemItem>();
			BlobContinuationToken continuationToken = null;
			if (!string.IsNullOrEmpty(dirKey))
				dirKey = dirKey + "/";
			CloudBlobDirectory dir = Container.GetDirectoryReference(dirKey);

			do
			{
				BlobResultSegment segmentResult = await dir.ListBlobsSegmentedAsync(continuationToken);
				continuationToken = segmentResult.ContinuationToken;
				foreach (IListBlobItem blob in segmentResult.Results)
				{
					ClientAzureFileSystemItem item = new ClientAzureFileSystemItem();
					string name = blob.Uri.Segments[blob.Uri.Segments.Length - 1];
					if (name == EmptyFolderDummyBlobName)
						continue;

					if (blob is CloudBlob)
					{
						var blockBlob = (CloudBlob)blob;
						item.Name = name;
						item.DateModified = blockBlob.Properties.LastModified.GetValueOrDefault().DateTime;
						item.Size = blockBlob.Properties.Length;
					}
					else if (blob is CloudBlobDirectory)
					{
						var subDir = (CloudBlobDirectory)blob;
						item.Name = name.Substring(0, name.Length - 1);
						item.IsDirectory = true;
						item.HasSubDirectories = await GetHasDirectories(subDir);
						item.DateModified = DateTime.UtcNow;
					}
					else
					{
						throw new Exception("Unsupported blob type");
					}
					result.Add(item);
				}
			} while (continuationToken != null);

			return result;
		}

		async Task<bool> GetHasDirectories(CloudBlobDirectory dir)
		{
			bool result;
			BlobContinuationToken continuationToken = null;
			do
			{
				BlobResultSegment segmentResult = await dir.ListBlobsSegmentedAsync(continuationToken);
				continuationToken = segmentResult.ContinuationToken;
				result = segmentResult.Results.Any(blob => blob is CloudBlobDirectory);
			} while (!result && continuationToken != null);
			return result;
		}

		public async Task CreateDirectoryAsync(string rootKey, string name)
		{
			string blobKey = $"{name}/{EmptyFolderDummyBlobName}";
			if (!string.IsNullOrEmpty(rootKey))
				blobKey = $"{rootKey}/{blobKey}";
			CloudBlockBlob dirBlob = Container.GetBlockBlobReference(blobKey);
			await dirBlob.UploadTextAsync("");
		}

		public async Task MoveUploadedFileAsync(FileInfo file, string destinationKey)
		{
			CloudBlockBlob newBlob = Container.GetBlockBlobReference(destinationKey);
			await newBlob.UploadFromFileAsync(file.FullName);
		}

		public async Task RemoveAsync(string key)
		{
			CloudBlob blob = Container.GetBlobReference(key);
			bool isFile = await blob.ExistsAsync();
			if (isFile)
				await RemoveFileAsync(blob);
			else
				await RemoveDirectoryAsync(key + "/");
		}

		async Task RemoveFileAsync(CloudBlob blob)
		{
			await blob.DeleteAsync();
		}

		async Task RemoveDirectoryAsync(string key)
		{
			CloudBlobDirectory dir = Container.GetDirectoryReference(key);
			await RemoveDirectoryAsync(dir);
		}

		async Task RemoveDirectoryAsync(CloudBlobDirectory dir)
		{
			var children = new List<IListBlobItem>();
			BlobContinuationToken continuationToken = null;

			do
			{
				BlobResultSegment segmentResult = await dir.ListBlobsSegmentedAsync(continuationToken);
				continuationToken = segmentResult.ContinuationToken;
				children.AddRange(segmentResult.Results);
			} while (continuationToken != null);

			foreach (IListBlobItem blob in children)
			{
				if (blob is CloudBlob)
				{
					await RemoveFileAsync((CloudBlob)blob);
				}
				else if (blob is CloudBlobDirectory)
				{
					await RemoveDirectoryAsync((CloudBlobDirectory)blob);
				}
				else
				{
					throw new Exception("Unsupported blob type");
				}
			}
		}

		public async Task MoveAsync(string sourceKey, string destinationKey)
		{
			await CopyAsync(sourceKey, destinationKey, true);
		}

		public async Task CopyAsync(string sourceKey, string destinationKey, bool deleteSource = false)
		{
			CloudBlob blob = Container.GetBlobReference(sourceKey);
			bool isFile = await blob.ExistsAsync();
			if (isFile)
				await CopyFileAsync(blob, destinationKey, deleteSource);
			else
				await CopyDirectoryAsync(sourceKey, destinationKey + "/", deleteSource);
		}

		async Task CopyFileAsync(CloudBlob blob, string destinationKey, bool deleteSource = false)
		{
			CloudBlob blobCopy = Container.GetBlobReference(destinationKey);
			await blobCopy.StartCopyAsync(blob.Uri);
			if (deleteSource)
				await blob.DeleteAsync();
		}

		async Task CopyDirectoryAsync(string sourceKey, string destinationKey, bool deleteSource = false)
		{
			CloudBlobDirectory dir = Container.GetDirectoryReference(sourceKey);
			await CopyDirectoryAsync(dir, destinationKey, deleteSource);
		}

		async Task CopyDirectoryAsync(CloudBlobDirectory dir, string destinationKey, bool deleteSource = false)
		{
			var children = new List<IListBlobItem>();
			BlobContinuationToken continuationToken = null;

			do
			{
				BlobResultSegment segmentResult = await dir.ListBlobsSegmentedAsync(continuationToken);
				continuationToken = segmentResult.ContinuationToken;
				children.AddRange(segmentResult.Results);
			} while (continuationToken != null);

			foreach (IListBlobItem blob in children)
			{
				string childCopyName = blob.Uri.Segments[blob.Uri.Segments.Length - 1];
				string childCopyKey = $"{destinationKey}{childCopyName}";
				if (blob is CloudBlob)
				{
					await CopyFileAsync((CloudBlob)blob, childCopyKey, deleteSource);
				}
				else if (blob is CloudBlobDirectory)
				{
					await CopyDirectoryAsync((CloudBlobDirectory)blob, childCopyKey, deleteSource);
				}
				else
				{
					throw new Exception("Unsupported blob type");
				}
			}
		}

		public async Task RenameAsync(string key, string newName)
		{
			string parentKey = key.EndsWith('/') ? key.Substring(0, key.Length - 1) : key;
			int index = parentKey.LastIndexOf('/');
			string newKey;
			if (index >= 0)
			{
				parentKey = parentKey.Substring(0, index + 1);
				newKey = parentKey + newName;
			}
			else
				newKey = newName;

			await CopyAsync(key, newKey, true);
		}

		#region IFileProvider implementation

		public void Copy(string sourceKey, string destinationKey)
		{
			CopyAsync(sourceKey, destinationKey).Wait();
		}

		public void CreateDirectory(string rootKey, string name)
		{
			CreateDirectoryAsync(rootKey, name).Wait();
		}

		public IList<IClientFileSystemItem> GetDirectoryContents(string dirKey)
		{
			return GetDirectoryContentsAsync(dirKey).GetAwaiter().GetResult();
		}

		public void Move(string sourceKey, string destinationKey)
		{
			MoveAsync(sourceKey, destinationKey).Wait();
		}

		public void MoveUploadedFile(FileInfo file, string destinationKey)
		{
			MoveUploadedFileAsync(file, destinationKey).Wait();
		}

		public void Remove(string key)
		{
			RemoveAsync(key).Wait();
		}

		public void RemoveUploadedFile(FileInfo file)
		{
			file.Delete();
		}

		public void Rename(string key, string newName)
		{
			RenameAsync(key, newName).Wait();
		}

		#endregion
	}

	public class ClientAzureFileSystemItem : IClientFileSystemItem
	{
		public ClientAzureFileSystemItem()
		{
			CustomFields = new Dictionary<string, object>();
		}
		public string Id { get; set; }
		public string Name { get; set; }
		public DateTime DateModified { get; set; }
		public bool IsDirectory { get; set; }
		public long Size { get; set; }
		public bool HasSubDirectories { get; set; }
		public IDictionary<string, object> CustomFields { get; }
	}
}
