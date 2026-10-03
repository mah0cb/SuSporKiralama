# SuSporKiralama — Proje Kuralları

Su sporları ekipman kiralama otomasyonu (NTP dersi dönem projesi). Yol haritası: `docs/PLAN.md`.

## Mimari
- Katmanlar: `Entities` ← `DataAccess` ← `Business` ← UI (Windows Forms, ileride). `YapayZeka` Business'ın yanında, öneri sağlayıcılarını içerir.
- Referans yönü tek yönlüdür: DataAccess → Entities; Business → DataAccess + Entities; YapayZeka → Business; UI → Business + YapayZeka (+ gerekirse Entities). Tersine referans eklenmez; Business yapay zeka sağlayıcılarını yalnızca `IAiOneriSaglayici` arayüzü üzerinden bilir.
- DevExpress paketleri, referansları ve kodu **yalnızca UI katmanında** olur.
- Class library'ler `net10.0`; UI projesi `net10.0-windows`.

## Kod
- Identifier'larda (sınıf, property, metot, değişken, dosya adı) Türkçe karakter kullanılmaz: `Musteri`, `Ucret`. Yorumlar ve kullanıcıya gösterilen metinler Türkçe ve doğru yazımla olur.
- Kod sade ve okunabilir olmalı; önemli yerlerde kısa Türkçe yorum bulunur (kod sunumda anlatılacak).
- Veritabanı değişikliği = yeni migration (`dotnet ef migrations add <Ad> -p src/SuSporKiralama.DataAccess`).

## Gizli bilgiler
- Parola, API anahtarı ve bağlantı dizesi repoya girmez. `appsettings.json` gitignore'dadır; repoda sadece `appsettings.example.json` bulunur.

## Süreç
- Küçük ve anlamlı commit'ler; mesajlar Türkçe ve açıklayıcı. Her değişiklikten sonra `dotnet build` alınır, iş biriktirilmez. Push'u proje sahibi yapar.
- Sadece aktif aşama uygulanır. Gelecek aşamalar yalnızca tasarım kararlarında dikkate alınır; ileri bir aşamaya ait bir şey gerekli görünürse kod yazılmaz, kullanıcıya sorulur.
- Her aşama bitince `docs/PLAN.md`'de o aşama "tamamlandı" olarak işaretlenir ve kısa not düşülür.
