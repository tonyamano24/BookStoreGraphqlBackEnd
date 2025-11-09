using System.Collections.Concurrent;
using BookStore.Api.Models;

namespace BookStore.Api.Data;

public class CatalogItemDetailProvider
{
    private readonly ICatalogRepository _repository;
    private readonly ConcurrentDictionary<Guid, CatalogItemDetail> _cache = new();

    public CatalogItemDetailProvider(ICatalogRepository repository)
    {
        _repository = repository;
    }

    public CatalogItemDetail GetDetail(Guid id)
    {
        var item = _repository.GetById(id) ?? throw new KeyNotFoundException($"Catalog item {id} was not found.");

        return _cache.GetOrAdd(id, _ => BuildDetail(item));
    }

    private static CatalogItemDetail BuildDetail(CatalogItem item)
    {
        var instructor = BuildInstructor(item);
        var modules = BuildModules(item);
        var pricing = BuildPricing(item);
        var outcomes = BuildOutcomes(item);
        var resources = BuildResources(item);

        return new CatalogItemDetail(item, instructor, modules, pricing, outcomes, resources);
    }

    private static InstructorProfile BuildInstructor(CatalogItem item)
    {
        var (name, bio, exp, avatar) = item.Category switch
        {
            CatalogItemCategory.Course => ("Dr. Supakorn Boonyarit", "GraphQL architect coaching enterprise teams to ship production-ready schemas.", 14, "https://placehold.co/160x160?text=Dr+Supakorn"),
            CatalogItemCategory.Book => ("Patcharee Wongchai", "Technical author focused on developer experience and documentation patterns.", 11, "https://placehold.co/160x160?text=Patcharee"),
            CatalogItemCategory.Merchandise => ("Studio Orbit", "Design studio crafting tactile learning aids for modern workshops.", 8, "https://placehold.co/160x160?text=Studio+Orbit"),
            _ => ("GraphQL Guild", "Community of practitioners curating advanced GraphQL practices.", 10, "https://placehold.co/160x160?text=GraphQL+Guild")
        };

        var expertise = item.Category switch
        {
            CatalogItemCategory.Course => new[] { "Schema Design", "Federation", "Performance" },
            CatalogItemCategory.Book => new[] { "Technical Writing", "DX Research", "Playbooks" },
            _ => new[] { "Workshop Ops", "Community", "Design Systems" }
        };

        var socials = new[]
        {
            new ResourceLink("Website", "https://example.com/" + name.Replace(" ", "").ToLowerInvariant(), "website"),
            new ResourceLink("LinkedIn", "https://www.linkedin.com/in/graphql-" + name.Split(' ')[0].ToLowerInvariant(), "linkedin"),
            new ResourceLink("GitHub", "https://github.com/" + name.Split(' ')[0].ToLowerInvariant() + "-graphql", "github")
        };

        return new InstructorProfile(name, bio, exp, expertise, avatar, socials);
    }

    private static IReadOnlyList<ContentModule> BuildModules(CatalogItem item)
    {
        var baseTitle = item.Category switch
        {
            CatalogItemCategory.Course => "Module",
            CatalogItemCategory.Book => "Chapter",
            _ => "Segment"
        };

        return new[]
        {
            new ContentModule(
                $"{baseTitle} 1: Intentional Schema Design",
                "Define contracts, entity boundaries, and naming conventions that scale.",
                LearningLevel.Intermediate,
                45,
                new[]
                {
                    "Set schema guardrails with linting",
                    "Model pagination and filtering",
                    "Align object graphs with product journeys"
                },
                new[]
                {
                    new ModuleResource("Schema Design Checklist", "https://resources.graphqlworkshop.dev/schema-checklist.pdf", "pdf"),
                    new ModuleResource("SDL playground", "https://graphql.gallery/sdl", "playground")
                }),
            new ContentModule(
                $"{baseTitle} 2: Data Fetching Strategies",
                "Compare resolvers, data loaders, and persisted operations with hands-on profiles.",
                LearningLevel.Advanced,
                60,
                new[]
                {
                    "Structure resolver pipelines",
                    "Use batching + caching effectively",
                    "Measure impact with tracing"
                },
                new[]
                {
                    new ModuleResource("Resolver Tracing Lab", "https://lab.graphqlworkshop.dev/tracing", "lab"),
                    new ModuleResource("Server performance template", "https://github.com/graphql-guild/perf-template", "github")
                }),
            new ContentModule(
                $"{baseTitle} 3: Experience Patterns",
                "Bring the schema to life with client patterns, documentation, and governance.",
                LearningLevel.Beginner,
                40,
                new[]
                {
                    "Craft API handoffs",
                    "Document change windows",
                    "Automate review workflows"
                },
                new[]
                {
                    new ModuleResource("DX Playbook", "https://resources.graphqlworkshop.dev/dx-playbook", "pdf"),
                    new ModuleResource("Change management demo", "https://youtu.be/dQw4w9WgXcQ", "video")
                })
        };
    }

    private static IReadOnlyList<PricingTier> BuildPricing(CatalogItem item)
    {
        return new[]
        {
            new PricingTier(
                "Solo",
                decimal.Round(item.Price, 2),
                "เข้าถึงคอนเทนต์ทั้งหมด + template สำหรับทำ workshop ส่วนตัว",
                new[]
                {
                    "ดาวน์โหลดสไลด์ + workbook",
                    "เข้าชม recording ย้อนหลัง 90 วัน"
                }),
            new PricingTier(
                "Team",
                decimal.Round(item.Price * 1.8m, 2),
                "ลิขสิทธิ์สูงสุด 8 seats พร้อม coaching Q&A",
                new[]
                {
                    "Live Q&A ส่วนตัว 60 นาที",
                    "Checklist สำหรับ rollout ในองค์กร",
                    "Priority email support"
                }),
            new PricingTier(
                "Enterprise",
                decimal.Round(item.Price * 3.2m, 2),
                "Custom engagement สำหรับองค์กรที่ต้องการ roadmapชัดเจน",
                new[]
                {
                    "Architecture review 2 sessions",
                    "Pair design กับทีม Core API",
                    "รายงาน maturity scorecard"
                })
        };
    }

    private static IReadOnlyList<string> BuildOutcomes(CatalogItem item)
    {
        return new[]
        {
            $"Deploy {item.Title} ด้วยแนวทางที่วัดผลได้",
            "ลดการซ้ำซ้อนของ resolver และ data loader",
            "ออกแบบ schema ที่รองรับการเติบโตของทีม",
            "วางมาตรฐานการสื่อสารระหว่าง Backend/Frontend"
        };
    }

    private static IReadOnlyList<ResourceLink> BuildResources(CatalogItem item)
    {
        return new[]
        {
            new ResourceLink("Architecture Decision Record Template", "https://resources.graphqlworkshop.dev/adr-template", "template"),
            new ResourceLink("Diagnostic Checklist", "https://resources.graphqlworkshop.dev/diagnostic-checklist", "checklist"),
            new ResourceLink($"{item.Category} Reference Repo", "https://github.com/graphql-guild/reference-" + item.Category.ToString().ToLowerInvariant(), "github")
        };
    }
}
