using SuSporKiralama.DataAccess.Guvenlik;

namespace SuSporKiralama.Tests;

public class GirisServisiTestleri
{
    // --- SifreHasher.Dogrula ---

    [Fact]
    public void SifreDogrula_DogruSifre_True()
    {
        Assert.True(SifreHasher.Dogrula("Guvenli123!", SifreHasher.Hashle("Guvenli123!")));
    }

    [Theory]
    [InlineData("guvenli123!")] // büyük/küçük harf duyarlı
    [InlineData("Guvenli123")]
    [InlineData("")]
    public void SifreDogrula_YanlisSifre_False(string sifre)
    {
        Assert.False(SifreHasher.Dogrula(sifre, SifreHasher.Hashle("Guvenli123!")));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("test-hash")]
    [InlineData("100000.bozuk.hash")]
    public void SifreDogrula_BozukHash_False(string? hash)
    {
        Assert.False(SifreHasher.Dogrula("Guvenli123!", hash));
    }
}
