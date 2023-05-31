using SignalDataPicker.model;
using System.Threading.Tasks;

namespace SignalDataPicker.service
{
    internal interface IFileService
    {
        Task<FileData?> LoadFile(FileType fileType);

        Task<bool> SaveFile(FileData fileData, OutputType outputType, DataAxis dataAxis, int startIndex, int endIndex);
    }
}
