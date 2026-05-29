public sealed class PetRepository(PetDbContext context, IDbConnection connection) : IPetRepository
{
    public async Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pet = await connection.QueryFirstOrDefaultAsync<Pet>(PetSqlConstants.GetByIdSql, new { Id = id });

        if (pet is null) return null;

        var photos = await connection.QueryAsync<PetPhoto>(PetSqlConstants.GetPhotosByPetIdSql, new { PetId = id });

        pet.Photos = photos.ToList();
        return pet;
    }

    public async Task<Pet?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var pet = await connection.QueryFirstOrDefaultAsync<Pet>(PetSqlConstants.GetBySlugSql, new { Slug = slug });
        if (pet is null) return null;
        var photos = await connection.QueryAsync<PetPhoto>(PetSqlConstants.GetPhotosByPetIdSql, new { PetId = pet.Id });
        pet.Photos = photos.ToList();
        return pet;
    }

    public async Task<IEnumerable<Pet>> GetAllAsync(CancellationToken cancellationToken = default)
        => await connection.QueryAsync<Pet>(PetSqlConstants.GetAllSql);

    public async Task<IEnumerable<Pet>> GetByOwnerAsync(
        Guid ownerId, EOwnerType ownerType, CancellationToken cancellationToken = default)
        => await connection.QueryAsync<Pet>(
            PetSqlConstants.GetByOwnerSql,
            new { OwnerId = ownerId, OwnerType = (int)ownerType });

    public async Task<IEnumerable<Pet>> GetNearbyAsync(
        decimal latitude, decimal longitude, double radiusKm, CancellationToken cancellationToken = default)
        => await connection.QueryAsync<Pet>(
            PetSqlConstants.GetNearbySql,
            new { Latitude = (double)latitude, Longitude = (double)longitude, RadiusKm = radiusKm });

    public async Task<Pet> CreateAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        context.Pets.Add(pet);
        await context.SaveChangesAsync(cancellationToken);
        return pet;
    }

    public async Task<Pet> UpdateAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        context.Pets.Update(pet);
        await context.SaveChangesAsync(cancellationToken);
        return pet;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pet = await context.Pets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (pet is null) return;
        context.Pets.Remove(pet);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddPhotoAsync(PetPhoto photo, CancellationToken cancellationToken = default)
    {
        context.PetPhotos.Add(photo);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletePhotoAsync(Guid photoId, CancellationToken cancellationToken = default)
    {
        var photo = await context.PetPhotos.FirstOrDefaultAsync(x => x.Id == photoId, cancellationToken);
        if (photo is null) return;
        context.PetPhotos.Remove(photo);
        await context.SaveChangesAsync(cancellationToken);
    }
}
