using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Services.Indexing.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Business;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Tests.Services.Tokenization;

public class OptionalTermsFilterTests
{
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly IInvertedIndexDocumentRetriever _indexDocumentRetriever;
    private readonly ITokenHandler _sut;

    public OptionalTermsFilterTests()
    {
        _indexRepository = Substitute.For<IInvertedIndexRepository>();
        _indexDocumentRetriever = Substitute.For<IInvertedIndexDocumentRetriever>();
        _sut = new OptionalTokenHandler(_indexRepository, _indexDocumentRetriever);
    }

    [Fact]
    public void Filter_ShouldReturnAllDocuments_WhenNoOptionalTermsExist()
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
            RequiredTerms = new List<string> { "good" },
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
    public void Filter_ShouldReturnMatchingDocuments_WhenOptionalTermsExist()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var documentId3 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var document3 = new ScoredDocument { DocumentId = documentId3, Score = 12 };
        var inputDocuments = new List<ScoredDocument> { document1, document2, document3 };

        var optionalTerms = new List<string> { "optional1", "optional2" };
        var query = new ParsedQuery()
        {
            RequiredTerms = new List<string>(),
            ExcludedTerms = new List<string>(),
            OptionalTerms = optionalTerms
        };

        var indexEntry1 = new InvertedIndex { Id = Guid.NewGuid(), Term = "optional1", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 } } };
        var indexEntry2 = new InvertedIndex { Id = Guid.NewGuid(), Term = "optional2", DocumentFrequency = new Dictionary<Guid, int> { { documentId2, 1 } } };
        var optionalIndices = new List<InvertedIndex> { indexEntry1, indexEntry2 };

        _indexRepository.SearchTerms(optionalTerms).Returns(optionalIndices);
        _indexDocumentRetriever.GetDocumentIds(indexEntry1).Returns(new List<Guid> { documentId1 });
        _indexDocumentRetriever.GetDocumentIds(indexEntry2).Returns(new List<Guid> { documentId2 });

        var expected = new List<ScoredDocument> { document1, document2 };

        // Act
        var result = _sut.FilterHandler(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Filter_ShouldReturnEmpty_WhenOptionalTermsNotFoundInIndex()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var inputDocuments = new List<ScoredDocument> { document1, document2 };

        var optionalTerms = new List<string> { "nonexistent" };
        var query = new ParsedQuery()
        {
            RequiredTerms = new List<string>(),
            ExcludedTerms = new List<string>(),
            OptionalTerms = optionalTerms
        };

        _indexRepository.SearchTerms(optionalTerms).Returns(new List<InvertedIndex>());

        var expected = new List<ScoredDocument>();

        // Act
        var result = _sut.FilterHandler(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Filter_ShouldReturnUniqueDocuments_WhenDocumentMatchesMultipleOptionalTerms()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var inputDocuments = new List<ScoredDocument> { document1, document2 };

        var optionalTerms = new List<string> { "optional1", "optional2" };
        var query = new ParsedQuery()
        {
            RequiredTerms = new List<string>(),
            ExcludedTerms = new List<string>(),
            OptionalTerms = optionalTerms
        };

        var indexEntry1 = new InvertedIndex { Id = Guid.NewGuid(), Term = "optional1", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 } } };
        var indexEntry2 = new InvertedIndex { Id = Guid.NewGuid(), Term = "optional2", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 2 } } };
        var optionalIndices = new List<InvertedIndex> { indexEntry1, indexEntry2 };

        _indexRepository.SearchTerms(optionalTerms).Returns(optionalIndices);
        _indexDocumentRetriever.GetDocumentIds(indexEntry1).Returns(new List<Guid> { documentId1 });
        _indexDocumentRetriever.GetDocumentIds(indexEntry2).Returns(new List<Guid> { documentId1 });

        var expected = new List<ScoredDocument> { document1 };

        // Act
        var result = _sut.FilterHandler(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expected);
        result.Should().HaveCount(1);
    }

    [Fact]
    public void Filter_ShouldExcludeNonMatchingDocuments_WhenOptionalTermsExist()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var documentId3 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var document3 = new ScoredDocument { DocumentId = documentId3, Score = 12 };
        var inputDocuments = new List<ScoredDocument> { document1, document2, document3 };

        var optionalTerms = new List<string> { "optional" };
        var query = new ParsedQuery()
        {
            RequiredTerms = new List<string>(),
            ExcludedTerms = new List<string>(),
            OptionalTerms = optionalTerms
        };

        var indexEntry = new InvertedIndex { Id = Guid.NewGuid(), Term = "optional", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 } } };
        var optionalIndices = new List<InvertedIndex> { indexEntry };

        _indexRepository.SearchTerms(optionalTerms).Returns(optionalIndices);
        _indexDocumentRetriever.GetDocumentIds(indexEntry).Returns(new List<Guid> { documentId1 });

        var expected = new List<ScoredDocument> { document1 };

        // Act
        var result = _sut.FilterHandler(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expected);
        result.Should().NotContain(d => d.DocumentId == documentId2 || d.DocumentId == documentId3);
    }
}