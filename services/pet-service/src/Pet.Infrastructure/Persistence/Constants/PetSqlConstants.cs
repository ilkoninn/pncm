public static class PetSqlConstants
{
    public const string PetColumns = """
        id AS "Id", name AS "Name", slug AS "Slug", species AS "Species", breed AS "Breed",
        age_months AS "AgeMonths", gender AS "Gender", size AS "Size",
        color AS "Color", description AS "Description",
        is_vaccinated AS "IsVaccinated", is_neutered AS "IsNeutered",
        status AS "Status", owner_id AS "OwnerId", owner_type AS "OwnerType",
        owner_first_name AS "OwnerFirstName", owner_last_name AS "OwnerLastName",
        city AS "City", latitude AS "Latitude", longitude AS "Longitude",
        created_at AS "CreatedAt", updated_at AS "UpdatedAt",
        is_active AS "IsActive", is_deleted AS "IsDeleted"
        """;

    public const string PhotoColumns = """
        id AS "Id", pet_id AS "PetId", media_id AS "MediaId", is_primary AS "IsPrimary"
        """;

    public const string GetByIdSql = $"SELECT {PetColumns} FROM pets WHERE id = @Id";

    public const string GetBySlugSql = $"SELECT {PetColumns} FROM pets WHERE slug = @Slug AND is_active = true AND is_deleted = false";

    public const string GetPhotosByPetIdSql = $"SELECT {PhotoColumns} FROM pet_photos WHERE pet_id = @PetId";

    public const string GetAllSql = $"SELECT {PetColumns} FROM pets WHERE is_active = true AND is_deleted = false AND status != 5";

    public const string GetByOwnerSql = $"SELECT {PetColumns} FROM pets WHERE owner_id = @OwnerId AND owner_type = @OwnerType AND is_active = true AND is_deleted = false";

    public const string GetNearbySql = $"""
        SELECT {PetColumns} FROM pets
        WHERE is_active = true AND is_deleted = false
          AND latitude IS NOT NULL AND longitude IS NOT NULL
          AND 6371 * acos(
                cos(radians(@Latitude)) * cos(radians(latitude::float8))
                * cos(radians(longitude::float8) - radians(@Longitude))
                + sin(radians(@Latitude)) * sin(radians(latitude::float8))
              ) <= @RadiusKm
        """;
}
