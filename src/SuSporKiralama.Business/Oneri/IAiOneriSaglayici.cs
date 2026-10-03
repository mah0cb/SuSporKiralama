namespace SuSporKiralama.Business.Oneri;

/// <summary>
/// Ham öneri üreten strateji (Strategy deseni): Claude, kural tabanlı ya da yedekli sağlayıcı.
/// OneriServisi hangisinin çalıştığını bilmez. Implementasyonlar SuSporKiralama.YapayZeka
/// projesindedir; Business o projeye referans vermez (bağımlılık arayüze doğru).
/// Dönen öneriye güvenilmez, OneriServisi doğrular.
/// </summary>
public interface IAiOneriSaglayici
{
    Task<HamOneri> OneriUretAsync(OneriGirdisi girdi, CancellationToken iptal = default);
}
