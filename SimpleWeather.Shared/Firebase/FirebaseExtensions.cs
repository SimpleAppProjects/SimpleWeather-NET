#if __IOS__
using System;
using System.Threading.Tasks;
using Firebase.CloudFirestore;
using Firebase.Storage;
using Foundation;

namespace SimpleWeather.Firebase;

public static class FirebaseExtensions
{
	public static Task WriteToFileAsync(this StorageReference storageRef, string fileUrl)
	{
        var tcs = new TaskCompletionSource();

        storageRef.WriteToFile(NSUrl.CreateFileUrl(fileUrl), (url, error) =>
        {
            if (error != null)
            {
                tcs.TrySetException(new NSErrorException(error));
            }
            else
            {
                tcs.TrySetResult();
            }
        });

        return tcs.Task;
    }

    public static Task<QuerySnapshot> GetDocumentsAsync(this Query query, FirestoreSource source)
    {
        var tcs = new TaskCompletionSource<QuerySnapshot>();

        query.GetDocuments(source, (snapshot, error) =>
        {
            if (error != null)
            {
                tcs.TrySetException(new NSErrorException(error));
            }
            else
            {
                tcs.TrySetResult(snapshot);
            }
        });

        return tcs.Task;
    }
}
#endif