using Moq;
using OncoTargetExplorer.Api.Models.UniProt;
using OncoTargetExplorer.Api.Services;
using Xunit;

namespace OncoTargetExplorer.Api.Tests;

public class ProteinServiceTests
{
    private static UniProtEntry BuildSampleEntry() => new(
        PrimaryAccession: "P04626",
        Organism: new UniProtOrganism("Homo sapiens", "Human"),
        ProteinDescription: new UniProtProteinDescription(
            new UniProtRecommendedName(new UniProtValue("Receptor tyrosine-protein kinase erbB-2"))),
        Genes: [new UniProtGene(new UniProtValue("ERBB2"))],
        Comments:
        [
            new UniProtComment("FUNCTION", [new UniProtValue("Protein tyrosine kinase.")], null, null),
            new UniProtComment(
                "SUBCELLULAR LOCATION",
                null,
                null,
                [new UniProtSubcellularLocationEntry(new UniProtValue("Cell membrane")),
                 new UniProtSubcellularLocationEntry(new UniProtValue("Cell membrane"))]),
            new UniProtComment("DISEASE", null, new UniProtDisease("Gastric cancer"), null),
            new UniProtComment("DISEASE", null, null, null),
        ],
        Sequence: new UniProtSequence(1255),
        UniProtKBCrossReferences:
        [
            new UniProtCrossReference("HGNC", "HGNC:3430"),
            new UniProtCrossReference("EMBL", "X03363"),
        ]);

    [Fact]
    public async Task SearchAsync_MapsUniProtEntriesToSummaries()
    {
        var uniProtClient = new Mock<IUniProtClient>();
        uniProtClient
            .Setup(c => c.SearchAsync("ERBB2", It.IsAny<CancellationToken>()))
            .ReturnsAsync([BuildSampleEntry()]);

        var service = new ProteinService(uniProtClient.Object);

        var results = await service.SearchAsync("ERBB2");

        var summary = Assert.Single(results);
        Assert.Equal("P04626", summary.Accession);
        Assert.Equal("ERBB2", summary.GeneName);
        Assert.Equal("Receptor tyrosine-protein kinase erbB-2", summary.ProteinName);
        Assert.Equal("Homo sapiens", summary.Organism);
    }

    [Fact]
    public async Task GetDetailAsync_MapsFunctionLocationDiseaseAndCrossReferences()
    {
        var uniProtClient = new Mock<IUniProtClient>();
        uniProtClient
            .Setup(c => c.GetByAccessionAsync("P04626", It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildSampleEntry());

        var service = new ProteinService(uniProtClient.Object);

        var detail = await service.GetDetailAsync("P04626");

        Assert.NotNull(detail);
        Assert.Equal("Protein tyrosine kinase.", detail!.FunctionSummary);
        Assert.Equal(["Cell membrane"], detail.SubcellularLocations);
        Assert.Equal(1255, detail.SequenceLength);
        Assert.Equal(["Gastric cancer"], detail.DiseaseAssociations);
        var crossReference = Assert.Single(detail.CrossReferences);
        Assert.Equal("HGNC", crossReference.Database);
        Assert.Equal("HGNC:3430", crossReference.Id);
    }

    [Fact]
    public async Task GetDetailAsync_ReturnsNull_WhenAccessionNotFound()
    {
        var uniProtClient = new Mock<IUniProtClient>();
        uniProtClient
            .Setup(c => c.GetByAccessionAsync("UNKNOWN", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UniProtEntry?)null);

        var service = new ProteinService(uniProtClient.Object);

        var detail = await service.GetDetailAsync("UNKNOWN");

        Assert.Null(detail);
    }

    [Fact]
    public async Task SearchAsync_PropagatesUpstreamFailures()
    {
        var uniProtClient = new Mock<IUniProtClient>();
        uniProtClient
            .Setup(c => c.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("UniProt is down"));

        var service = new ProteinService(uniProtClient.Object);

        await Assert.ThrowsAsync<HttpRequestException>(() => service.SearchAsync("ERBB2"));
    }
}
