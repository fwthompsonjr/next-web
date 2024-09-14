using Bogus;
using next.core.implementations;
using next.core.interfaces;
using Moq;

namespace next.core.tests.implementations
{
    public class MailPersistenceTests
    {
        [Fact]
        public void PersistenceCanClearFolders()
        {
            var problem = Record.Exception(() =>
            {
                var sut = new MailPersistence(null);
                sut.Clear();
            });
            Assert.Null(problem);
        }
        [Fact]
        public void PersistenceCanSave()
        {
            var problem = Record.Exception(() =>
            {
                var faker = new Faker();
                var json = faker.Lorem.Paragraph();
                var mock = new Mock<IFileInteraction>(MockBehavior.Loose);
                var sut = new MailPersistence(mock.Object);
                sut.Save(json);
            });
            Assert.Null(problem);
        }

        [Fact]
        public void PersistenceCanSaveId()
        {
            var problem = Record.Exception(() =>
            {
                var faker = new Faker();
                var id = faker.Random.AlphaNumeric(10);
                var json = faker.Lorem.Paragraph();
                var mock = new Mock<IFileInteraction>(MockBehavior.Loose);
                var sut = new MailPersistence(mock.Object);
                sut.Save(id, json);
            });
            Assert.Null(problem);
        }
        [Fact]
        public void PersistenceCanDoItemExist()
        {
            var problem = Record.Exception(() =>
            {
                var faker = new Faker();
                var id = faker.Random.AlphaNumeric(10);
                var mock = new Mock<IFileInteraction>(MockBehavior.Loose);
                var sut = new MailPersistence(mock.Object);
                _ = sut.DoesItemExist(id);
            });
            Assert.Null(problem);
        }

        [Fact]
        public void PersistenceCanFetch()
        {
            var problem = Record.Exception(() =>
            {
                var mock = new Mock<IFileInteraction>(MockBehavior.Loose);
                var sut = new MailPersistence(mock.Object);
                _ = sut.Fetch();
            });
            Assert.Null(problem);
        }

        [Fact]
        public void PersistenceCanFetchId()
        {
            var problem = Record.Exception(() =>
            {
                var faker = new Faker();
                var id = faker.Random.AlphaNumeric(10);
                var mock = new Mock<IFileInteraction>(MockBehavior.Loose);
                var sut = new MailPersistence(mock.Object);
                _ = sut.Fetch(id);
            });
            Assert.Null(problem);
        }
    }
}
