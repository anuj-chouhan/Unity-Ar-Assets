using System;
using System.IO;
using System.Threading.Tasks;
using UnityGLTF.Loader;

public class MemoryLoader : IDataLoader
{
    private byte[] data;

    public MemoryLoader(byte[] data)
    {
        this.data = data;
    }

    public Stream LoadStream(string relativeFilePath)
    {
        return new MemoryStream(data);
    }

    public void LoadStream(string relativeFilePath, Action<Stream> onComplete, Action<float> onProgress = null)
    {
        onComplete?.Invoke(new MemoryStream(data));
    }

    public Task<Stream> LoadStreamAsync(string relativeFilePath)
    {
        return Task.FromResult<Stream>(new MemoryStream(data));
    }
}
