using NUnit.Framework;

namespace SwiftRideBookingBackend.Tests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        [Test]
        public void AssertThat_IsNotNull_Works()
        {
            object obj = new object();
            Assert.That(obj, Is.Not.Null);
        }

        [Test]
        public void AssertThat_AreEqual_Works()
        {
            int expected = 10;
            int actual = 5 + 5;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
