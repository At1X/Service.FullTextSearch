using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Services.Search.Business;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Tests.Services.Search;

public class SearchFilterAggregatorTests
{
    [Fact]
    public void Filter_ShouldApplyAllFiltersInSequence()
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
            RequiredTerms = new List<string> { "required" },
            ExcludedTerms = new List<string> { "excluded" },
            OptionalTerms = new List<string> { "optional" }
        };

        var filter1 = Substitute.For<ITokenHandler>();
        var filter2 = Substitute.For<ITokenHandler>();
        var filter3 = Substitute.For<ITokenHandler>();

        var afterFilter1 = new List<ScoredDocument> { document1, document2 };
        var afterFilter2 = new List<ScoredDocument> { document1 };
        var afterFilter3 = new List<ScoredDocument> { document1 };

        filter1.FilterHandler(inputDocuments, query).Returns(afterFilter1);
        filter2.FilterHandler(afterFilter1, query).Returns(afterFilter2);
        filter3.FilterHandler(afterFilter2, query).Returns(afterFilter3);

        var filters = new List<ITokenHandler> { filter1, filter2, filter3 };
        var sut = new SearchFilterAggregator(filters);

        // Act
        var result = sut.Filter(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(afterFilter3);
        filter1.Received(1).FilterHandler(inputDocuments, query);
        filter2.Received(1).FilterHandler(afterFilter1, query);
        filter3.Received(1).FilterHandler(afterFilter2, query);
    }

    [Fact]
    public void Filter_ShouldReturnOriginalDocuments_WhenNoFiltersProvided()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var inputDocuments = new List<ScoredDocument> { document1, document2 };

        var query = new ParsedQuery()
        {
            RequiredTerms = new List<string>(),
            ExcludedTerms = new List<string>(),
            OptionalTerms = new List<string>()
        };

        var filters = new List<ITokenHandler>();
        var sut = new SearchFilterAggregator(filters);

        // Act
        var result = sut.Filter(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(inputDocuments);
    }

    [Fact]
    public void Filter_ShouldApplySingleFilter_WhenOnlyOneFilterProvided()
    {
        // Arrange
        var documentId1 = Guid.NewGuid();
        var documentId2 = Guid.NewGuid();
        var document1 = new ScoredDocument { DocumentId = documentId1, Score = 10 };
        var document2 = new ScoredDocument { DocumentId = documentId2, Score = 11 };
        var inputDocuments = new List<ScoredDocument> { document1, document2 };

        var query = new ParsedQuery()
        {
            RequiredTerms = new List<string> { "term" },
            ExcludedTerms = new List<string>(),
            OptionalTerms = new List<string>()
        };

        var filter = Substitute.For<ITokenHandler>();
        var expectedResult = new List<ScoredDocument> { document1 };
        filter.FilterHandler(inputDocuments, query).Returns(expectedResult);

        var filters = new List<ITokenHandler> { filter };
        var sut = new SearchFilterAggregator(filters);

        // Act
        var result = sut.Filter(inputDocuments, query);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        filter.Received(1).FilterHandler(inputDocuments, query);
    }
}