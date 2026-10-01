namespace SuSporKiralama.Business.Guvenlik;

public interface IGirisServisi
{
    /// <summary>
    /// Şifreyi doğrular ve oturumu açar. Başarısız girişte GirisBasarisizException fırlatır;
    /// art arda 5 hatalı denemeden sonra o kullanıcı adı 5 dakika kilitlenir.
    /// </summary>
    void GirisYap(string kullaniciAdi, string sifre);

    void CikisYap();
}
