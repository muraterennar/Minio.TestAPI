# Minio.TestAPI

MinIO nesne depolama sistemiyle ASP.NET Core entegrasyonunu gösteren bir test/demo REST API projesidir. Dosya yükleme, görüntüleme ve indirme işlemlerini S3-uyumlu bir altyapı üzerinde nasıl gerçekleştireceğinizi öğrenmek için kullanabilirsiniz.

## Teknolojiler

| Katman | Teknoloji |
|---|---|
| Framework | .NET 10 / ASP.NET Core |
| Depolama | MinIO SDK v7 (S3-uyumlu) |
| Konteyner | Docker & Docker Compose |
| Dokümantasyon | OpenAPI / Swagger |

## Özellikler

- Yerel diskten MinIO bucket'ına dosya yükleme
- Tarayıcıda görüntüleme için geçici (presigned) bağlantı oluşturma
- Dosyayı doğrudan HTTP stream olarak indirme
- Docker Compose ile tek komutta ayağa kaldırma

## Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) & Docker Compose

## Kurulum ve Çalıştırma

### Docker Compose ile (önerilen)

```bash
git clone https://github.com/muraterennar/Minio.TestAPI.git
cd Minio.TestAPI
docker compose up --build
```

Servisler ayağa kalktıktan sonra:

| Servis | Adres |
|---|---|
| API | http://localhost:8080 |
| MinIO S3 | http://localhost:9000 |
| MinIO Konsol | http://localhost:9001 |

### Yerel Geliştirme

```bash
cd API
dotnet run
```

API varsayılan olarak `http://localhost:5144` adresinde çalışır.

## API Uç Noktaları

### `POST /file/upload`

Dosyayı MinIO'ya yükler.

| Parametre | Tür | Açıklama |
|---|---|---|
| `filePath` | query | Sunucu üzerindeki dosyanın tam yolu |

**Örnek:**
```
POST /file/upload?filePath=/home/user/resim.png
```

---

### `GET /file/view-link`

Dosyayı tarayıcıda görüntülemek için 1 saatlik geçici bağlantı döndürür.

| Parametre | Tür | Açıklama |
|---|---|---|
| `fileName` | query | Bucket içindeki dosya adı |

**Yanıt:**
```json
{ "downloadUrl": "http://localhost:9000/resimler/resim.png?..." }
```

---

### `GET /file/download-direct/{fileName}`

Dosyayı doğrudan HTTP yanıtı olarak stream eder.

| Parametre | Tür | Açıklama |
|---|---|---|
| `fileName` | path | Bucket içindeki dosya adı |

## Yapılandırma

MinIO bağlantı bilgileri şu an `FileService.cs` içinde sabit kodlanmış durumdadır:

```
Endpoint : localhost:9000
Kullanıcı: admin
Şifre    : password123
Bucket   : resimler
```

> **Uyarı:** Üretim ortamında bu bilgileri ortam değişkenleri veya güvenli bir yapılandırma yöneticisi aracılığıyla sağlayın.

## Proje Yapısı

```
Minio.TestAPI/
├── API/
│   ├── Controllers/
│   │   └── FileController.cs       # Dosya işlemleri endpoint'leri
│   ├── Services/
│   │   └── FileService.cs          # MinIO istemci sarmalayıcısı
│   ├── Program.cs                  # Uygulama başlangıç yapılandırması
│   ├── appsettings.json
│   └── Dockerfile
├── compose.yaml                    # Docker Compose tanımı
└── Minio.TestAPI.slnx
```

## Lisans

Bu proje eğitim amaçlı geliştirilmiştir.
