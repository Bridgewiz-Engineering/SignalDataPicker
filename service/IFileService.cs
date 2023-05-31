using SignalDataPicker.model;
using System.Threading.Tasks;

namespace SignalDataPicker.service
{
    internal interface IFileService
    {
        Task<FileData?> LoadFile(FileType fileType);
    }
}
