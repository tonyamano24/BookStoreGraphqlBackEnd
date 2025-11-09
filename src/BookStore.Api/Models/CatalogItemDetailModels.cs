using System.Collections.Generic;

namespace BookStore.Api.Models;

public enum LearningLevel
{
    Beginner,
    Intermediate,
    Advanced
}

public record ResourceLink(string Title, string Url, string Type);

public record ModuleResource(string Title, string Url, string Kind);

public record ContentModule(
    string Title,
    string Summary,
    LearningLevel Level,
    int DurationMinutes,
    IReadOnlyList<string> Objectives,
    IReadOnlyList<ModuleResource> Resources);

public record PricingTier(
    string Name,
    decimal Price,
    string Description,
    IReadOnlyList<string> Benefits);

public record InstructorProfile(
    string FullName,
    string Bio,
    int YearsOfExperience,
    IReadOnlyList<string> Expertise,
    string AvatarUrl,
    IReadOnlyList<ResourceLink> SocialLinks);

public record CatalogItemDetail(
    CatalogItem Item,
    InstructorProfile Instructor,
    IReadOnlyList<ContentModule> Modules,
    IReadOnlyList<PricingTier> PricingTiers,
    IReadOnlyList<string> Outcomes,
    IReadOnlyList<ResourceLink> RecommendedResources);
