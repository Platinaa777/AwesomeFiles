using AutoMapper;
using AwesomeFiles.Application.Commands.StartArchiveProcess;
using AwesomeFiles.HttpModels.Requests;

namespace AwesomeFiles.Api.Mapping;

public class ProcessProfile : Profile
{
    public ProcessProfile()
    {
        CreateMap<ListArchiveFiles, StartArchiveProcessCommand>()
            .ForCtorParam(nameof(StartArchiveProcessCommand.Files), opt => opt.MapFrom(src => src.FileNames));
    }
}