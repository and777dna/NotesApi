namespace NotesApi.Application.MapperDto.Interfaces;

public interface IMapper<in TEntity, out TDto>
{
    TDto ToDto(TEntity entity);
}
