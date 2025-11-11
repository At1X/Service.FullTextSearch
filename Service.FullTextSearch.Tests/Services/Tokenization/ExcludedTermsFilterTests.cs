using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.Services.Indexing.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Business;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Tests.Services.Tokenization;


public class ExcludedTermsFilterTests
{
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly IInvertedIndexDocumentRetriever _indexDocumentRetriever;
    private readonly ITokenHandler _sut;

    public ExcludedTermsFilterTests()
    {
        _indexRepository = Substitute.For<IInvertedIndexRepository>();
        _indexDocumentRetriever = Substitute.For<IInvertedIndexDocumentRetriever>();
        _sut = new ExcludedTokenHandler(_indexRepository, _indexDocumentRetriever);
    }

    [Fact]
    public void Filter_ShouldReturnFilteredDocuments_WhenExcludedTermsExist()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var documentId3 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var document3 = new ScoredDocument { DocumentId = documentId3, Score = 12 };
        var inputDocuments = new List<ScoredDocument> { document1, document2, document3 };

        var excludedTerms = new List<string> { "bad", "excluded" };
        var query = new ParsedQuery()
        {
            RequiredTerms = new List<string> { "good" },
            ExcludedTerms = excludedTerms,
            OptionalTerms = new List<string>()
        };

        var indexEntry1 = new InvertedIndex { Id = Guid.NewGuid(), Term = "bad", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 } } };
        var indexEntry2 = new InvertedIndex { Id = Guid.NewGuid(), Term = "excluded", DocumentFrequency = new Dictionary<Guid, int> { { documentId3, 1 } } };
        var excludedIndices = new List<InvertedIndex> { indexEntry1, indexEntry2 };

        _indexRepository.SearchTerms(excludedTerms).Returns(excludedIndices);
        _indexDocumentRetriever.GetDocumentIds(indexEntry1).Returns(new List<Guid> { documentId1 });
        _indexDocumentRetriever.GetDocumentIds(indexEntry2).Returns(new List<Guid> { documentId3 });

        var expected = new List<ScoredDocument> { document2 };

        // Act
        var result = _sut.FilterHandler(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Filter_ShouldReturnAllDocuments_WhenExcludedTermsoesNotExist()
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


        _indexRepository.SearchTerms(new List<string>()).Returns(new List<InvertedIndex>());

        var expected = new List<ScoredDocument> { document1, document2, document3 };

        // Act
        var result = _sut.FilterHandler(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void Filter_ShouldReturnEmpty_WhenAllDocumentsAreExcluded()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var documentId3 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var document3 = new ScoredDocument { DocumentId = documentId3, Score = 12 };
        var inputDocuments = new List<ScoredDocument> { document1, document2, document3 };

        var excludedTerms = new List<string> { "bad", "excluded" };
        var query = new ParsedQuery()
        {
            RequiredTerms = new List<string> { "good" },
            ExcludedTerms = excludedTerms,
            OptionalTerms = new List<string>()
        };

        var indexEntry1 = new InvertedIndex { Id = Guid.NewGuid(), Term = "bad", DocumentFrequency = new Dictionary<Guid, int> { { documentId1, 1 } } };
        var indexEntry2 = new InvertedIndex { Id = Guid.NewGuid(), Term = "bad", DocumentFrequency = new Dictionary<Guid, int> { { documentId2, 1 } } };
        var indexEntry3 = new InvertedIndex { Id = Guid.NewGuid(), Term = "excluded", DocumentFrequency = new Dictionary<Guid, int> { { documentId3, 1 } } };
        var excludedIndices = new List<InvertedIndex> { indexEntry1, indexEntry2, indexEntry3 };

        _indexRepository.SearchTerms(excludedTerms).Returns(excludedIndices);
        _indexDocumentRetriever.GetDocumentIds(indexEntry1).Returns(new List<Guid> { documentId1 });
        _indexDocumentRetriever.GetDocumentIds(indexEntry2).Returns(new List<Guid> { documentId2 });
        _indexDocumentRetriever.GetDocumentIds(indexEntry3).Returns(new List<Guid> { documentId3 });

        var expected = new List<ScoredDocument>();

        // Act
        var result = _sut.FilterHandler(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    
    
}