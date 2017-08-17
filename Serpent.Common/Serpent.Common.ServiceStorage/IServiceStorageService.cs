namespace Serpent.Common.ServiceStorage
{
    using System;
    using System.Threading.Tasks;

    public interface IServiceStorageService<TServiceStorageType>
    {
        Task<TServiceStorageType> GetStorageAsync();

        Task UpdateStorageAsync(TServiceStorageType storage);

        Task UpdateStorageAsync(Func<TServiceStorageType, bool> updateFunc);
    }
}
