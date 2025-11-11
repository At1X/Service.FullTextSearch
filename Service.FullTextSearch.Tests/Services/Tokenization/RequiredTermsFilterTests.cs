using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Services.Indexing.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Business;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Tests.Services.Tokenization;

public class RequiredTermsFilterTests
{
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly IInvertedIndexDocumentRetriever _indexDocumentRetriever;
    private readonly ITokenHandler _sut;

    public RequiredTermsFilterTests()
    {
        _indexRepository = Substitute.For<IInvertedIndexRepository>();
        _indexDocumentRetriever = Substitute.For<IInvertedIndexDocumentRetriever>();
        _sut = new RequiredTokenHandler(_indexRepository, _indexDocumentRetriever);
    }

    [Fact]
    public void Filter_ShouldReturnAllDocuments_WhenNoRequiredTermsExist()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var documentId3 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var document3 = new ScoredDocument { DocumentId = documentId3, Score = 12 };
        var inputDocuments = new List<ScoredDocument> { document1, document2, document3 };

        var query = new ParsedQuery()
        {
            RequiredTerms = new List<string>(),
            ExcludedTerms = new List<string>(),
            OptionalTerms = new List<string>()
        };

        var expected = new List<ScoredDocument> { document1, document2, document3 };

        // Act
        var result = _sut.FilterHandler(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Filter_ShouldReturnDocumentsContainingAllRequiredTerms_WhenMultipleRequiredTermsExist()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var documentId3 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var document3 = new ScoredDocument { DocumentId = documentId3, Score = 12 };
        var inputDocuments = new List<ScoredDocument> { document1, document2, document3 };

        var requiredTerms = new List<string> { "term1", "term2" };
        var query = new ParsedQuery()
        {
            RequiredTerms = requiredTerms,
            ExcludedTerms = new List<string>(),
            OptionalTerms = new List<string>()
        };

        var indexEntry1 = new InvertedIndex { Id = Guid.NewGuid(), Term = "term1", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 }, { documentId2, 1 } } };
        var indexEntry2 = new InvertedIndex { Id = Guid.NewGuid(), Term = "term2", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 }, { documentId3, 1 } } };
        var requiredIndices = new List<InvertedIndex> { indexEntry1, indexEntry2 };

        _indexRepository.SearchTerms(requiredTerms).Returns(requiredIndices);
        _indexDocumentRetriever.GetDocumentIds(indexEntry1).Returns(new List<Guid> { documentId1, documentId2 });
        _indexDocumentRetriever.GetDocumentIds(indexEntry2).Returns(new List<Guid> { documentId1, documentId3 });

        var expected = new List<ScoredDocument> { document1 };

        // Act
        var result = _sut.FilterHandler(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Filter_ShouldReturnMatchingDocuments_WhenSingleRequiredTermExists()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var documentId3 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var document3 = new ScoredDocument { DocumentId = documentId3, Score = 12 };
        var inputDocuments = new List<ScoredDocument> { document1, document2, document3 };

        var requiredTerms = new List<string> { "required" };
        var query = new ParsedQuery()
        {
            RequiredTerms = requiredTerms,
            ExcludedTerms = new List<string>(),
            OptionalTerms = new List<string>()
        };

        var indexEntry = new InvertedIndex { Id = Guid.NewGuid(), Term = "required", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 }, { documentId2, 1 } } };
        var requiredIndices = new List<InvertedIndex> { indexEntry };

        _indexRepository.SearchTerms(requiredTerms).Returns(requiredIndices);
        _indexDocumentRetriever.GetDocumentIds(indexEntry).Returns(new List<Guid> { documentId1, documentId2 });

        var expected = new List<ScoredDocument> { document1, document2 };

        // Act
        var result = _sut.FilterHandler(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Filter_ShouldReturnEmpty_WhenNoDocumentsContainAllRequiredTerms()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var documentId3 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var document3 = new ScoredDocument { DocumentId = documentId3, Score = 12 };
        var inputDocuments = new List<ScoredDocument> { document1, document2, document3 };

        var requiredTerms = new List<string> { "term1", "term2" };
        var query = new ParsedQuery()
        {
            RequiredTerms = requiredTerms,
            ExcludedTerms = new List<string>(),
            OptionalTerms = new List<string>()
        };

        var indexEntry1 = new InvertedIndex { Id = Guid.NewGuid(), Term = "term1", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 } } };
        var indexEntry2 = new InvertedIndex { Id = Guid.NewGuid(), Term = "term2", DocumentFrequency = new Dictionary<Guid, int> { { documentId2, 1 } } };
        var requiredIndices = new List<InvertedIndex> { indexEntry1, indexEntry2 };

        _indexRepository.SearchTerms(requiredTerms).Returns(requiredIndices);
        _indexDocumentRetriever.GetDocumentIds(indexEntry1).Returns(new List<Guid> { documentId1 });
        _indexDocumentRetriever.GetDocumentIds(indexEntry2).Returns(new List<Guid> { documentId2 });

        var expected = new List<ScoredDocument>();

        // Act
        var result = _sut.FilterHandler(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Filter_ShouldReturnCorrectDocuments_WhenThreeRequiredTermsExist()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var documentId3 = Guid.NewGuid();
        var documentId4 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var document3 = new ScoredDocument { DocumentId = documentId3, Score = 12 };
        var document4 = new ScoredDocument { DocumentId = documentId4, Score = 13 };
        var inputDocuments = new List<ScoredDocument> { document1, document2, document3, document4 };

        var requiredTerms = new List<string> { "term1", "term2", "term3" };
        var query = new ParsedQuery()
        {
            RequiredTerms = requiredTerms,
            ExcludedTerms = new List<string>(),
            OptionalTerms = new List<string>()
        };

        var indexEntry1 = new InvertedIndex { Id = Guid.NewGuid(), Term = "term1", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 }, { documentId2, 1 }, { documentId3, 1 } } };
        var indexEntry2 = new InvertedIndex { Id = Guid.NewGuid(), Term = "term2", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 }, { documentId2, 1 } } };
        var indexEntry3 = new InvertedIndex { Id = Guid.NewGuid(), Term = "term3", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 } } };
        var requiredIndices = new List<InvertedIndex> { indexEntry1, indexEntry2, indexEntry3 };

        _indexRepository.SearchTerms(requiredTerms).Returns(requiredIndices);
        _indexDocumentRetriever.GetDocumentIds(indexEntry1).Returns(new List<Guid> { documentId1, documentId2, documentId3 });
        _indexDocumentRetriever.GetDocumentIds(indexEntry2).Returns(new List<Guid> { documentId1, documentId2 });
        _indexDocumentRetriever.GetDocumentIds(indexEntry3).Returns(new List<Guid> { documentId1 });

        var expected = new List<ScoredDocument> { document1 };

        // Act
        var result = _sut.FilterHandler(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}