# BookStore GraphQL Backend

โปรเจกต์นี้เป็นตัวอย่าง Backend สำหรับสอนการเรียกใช้งาน GraphQL API ด้วย .NET 8 (C#) สำหรับเว็บ/แอปที่ต้องการจัดการ "รายการหนังสือ-คอร์ส-สินค้า" โดยเน้นให้ฝั่ง Front-end ได้ฝึกฝนการเขียน Query, Mutation, การจัดการ state, loader และ error handling

## ไฮไลท์ของระบบ
- **Hot Chocolate GraphQL Server** พร้อมเปิดใช้งาน Filtering/Sorting ใน resolver เดียวกัน
- **Repository แบบสลับได้**: ใช้ In-memory สำหรับ workshop หรือ Postgres ผ่าน `NpgsqlDataSource` เมื่อมี connection string
- **Type Extension ของ `CatalogItem`** เติม field `categoryDisplayName` และ `durationHours` ที่คำนวณจากข้อมูลจริง
- **Seeder อัตโนมัติ** ทั้งในหน่วยความจำและฐานข้อมูลจริง ทำให้ demo ได้ทันทีหลังรัน
- **Docker Compose** ให้ Postgres + API ทำงานร่วมกันรวดเร็ว เหมาะกับ session ที่ต้องการ environment แบบเดียวกันทุกเครื่อง
- **Image URL** สำหรับโชว์ตัวอย่างปกหนังสือ/สินค้าใน UI ได้เลยทันทีจาก GraphQL
- **GraphQL Pagination** มี query `catalogItemsPage` สำหรับเลื่อนหน้าทั้งแบบกรอง category และได้ total count

## โครงสร้างโปรเจกต์

```
BookStoreGraphqlBackEnd/
├── src/
│   └── BookStore.Api/
│       ├── BookStore.Api.csproj
│       ├── Program.cs
│       ├── Data/
│       ├── GraphQL/
│       └── Models/
└── readme.md (ไฟล์นี้)
```

### เทคโนโลยีหลัก
- **.NET 8 Minimal API**
- **Hot Chocolate GraphQL Server** สำหรับให้บริการ GraphQL endpoint
- **In-memory repository** เพื่อเก็บข้อมูลจำลอง (mock) ของสินค้า/คอร์ส/หนังสือ

## การเตรียมสภาพแวดล้อม
1. ติดตั้ง [.NET 8 SDK](https://dotnet.microsoft.com/download)
2. ติดตั้ง PostgreSQL (ทดสอบแล้วกับเวอร์ชัน 15+) หรือเตรียม Docker Engine สำหรับใช้ docker-compose
3. ตั้งค่า connection string ใน `appsettings.json` หรือ environment variable `POSTGRES_CONNECTION_STRING` เช่น  
   `Host=localhost;Port=5432;Database=bookstore;Username=bookstore_app;Password=change-me`
4. เปิดเทอร์มินัลที่โฟลเดอร์โปรเจกต์นี้แล้วรัน
   ```bash
   dotnet restore ./src/BookStore.Api/BookStore.Api.csproj
   dotnet build ./src/BookStore.Api/BookStore.Api.csproj -c Release
   dotnet run --project ./src/BookStore.Api/BookStore.Api.csproj
   ```
5. เซิร์ฟเวอร์จะเปิดที่ `http://localhost:5000` (หรือพอร์ตที่กำหนดใน `ASPNETCORE_URLS`) และ GraphQL Playground/Schema explorer เข้าผ่าน `http://localhost:5000/graphql`

### โหมดฐานข้อมูล (In-Memory vs Postgres)
- `Program.cs` จะพยายามอ่าน connection string จาก `ConnectionStrings:CatalogDb` หรือ `POSTGRES_CONNECTION_STRING`
- ถ้ามีค่า ระบบจะสร้าง `NpgsqlDataSource` แล้วเลือก `PostgresCatalogRepository` โดยอัตโนมัติ พร้อมสร้างตาราง `catalog_items` และ seed ข้อมูล 3 รายการแรก
- หากไม่ระบุ connection string ระบบจะ fallback ไปยัง `InMemoryCatalogRepository` เพื่อให้ workshop เริ่มได้ทันทีโดยไม่ต้องมีฐานข้อมูล

### Hot reload สำหรับพัฒนา
```bash
dotnet watch run --project ./src/BookStore.Api/BookStore.Api.csproj
```
คำสั่งนี้ช่วยให้สอนได้ต่อเนื่อง ระหว่างแก้ Query/Mutation แล้วปล่อยให้ Hot Chocolate reload ให้อัตโนมัติ

> **หมายเหตุเรื่องการทดสอบ:** ตอนนี้ยังไม่มี test project ใต้ `src/` หากต้องการเพิ่ม xUnit + snapshot test ให้สร้างโฟลเดอร์ใหม่แล้วเชื่อมต่อกับ solution ตามแนวทางใน `AGENTS.md`

### ตัวอย่างการเปิด Postgres แบบรวดเร็วด้วย Docker
```bash
docker run --name bookstore-postgres \
  -e POSTGRES_USER=bookstore_app \
  -e POSTGRES_PASSWORD=change-me \
  -e POSTGRES_DB=bookstore \
  -p 5432:5432 \
  -d postgres:16
```
เมื่อ API เชื่อมต่อสำเร็จ ตาราง `catalog_items` จะถูกสร้างให้อัตโนมัติพร้อม seed ข้อมูลตัวอย่าง 3 รายการ

### ใช้งานด้วย Docker Compose (API + Postgres)
1. สร้างอิมเมจและเปิดทุกบริการ
   ```bash
   docker compose up --build -d
   ```
2. รอ healthcheck ของ Postgres แล้วเข้า `http://localhost:8081/graphql`
3. ดู log ของฝั่ง API
   ```bash
   docker compose logs -f api
   ```
4. ปิดและลบคอนเทนเนอร์
   ```bash
   docker compose down
   ```
คำสั่งนี้ใช้ volume `pgdata` เก็บข้อมูลถาวร หากต้องการล้างฐานข้อมูลให้รัน `docker compose down -v`

## สคีมาของ GraphQL
- `CatalogItem`
  - `id: ID!`
  - `title: String!`
  - `description: String`
  - `category: CatalogItemCategory!`
  - `price: Decimal!`
  - `durationMinutes: Int!`
  - `imageUrl: String`
  - `createdAtUtc: DateTime!`
  - **ฟิลด์เสริมจาก type extension**
    - `categoryDisplayName: String!`
    - `durationHours: Float!`
- `CatalogItemCategory` (enum): `BOOK`, `COURSE`, `MERCHANDISE`

- `CatalogItemsPage`
  - `items: [CatalogItem!]!`
  - `totalCount: Int!`
  - `page: Int!`
  - `pageSize: Int!`
  - `totalPages: Int!`
  - `hasPreviousPage: Boolean!`
  - `hasNextPage: Boolean!`

### Query ตัวอย่าง
#### 1. ดึงรายการทั้งหมด (พร้อมตัวกรอง category)
```graphql
query GetCatalog($category: CatalogItemCategory) {
  catalogItems(category: $category) {
    id
    title
    category
    categoryDisplayName
    imageUrl
    price
  }
}
```
ตัวอย่างตัวแปร:
```json
{
  "category": "COURSE"
}
```

#### 2. ดึงข้อมูลแบบแบ่งหน้า
```graphql
query GetCatalogPage($category: CatalogItemCategory, $page: Int = 1, $pageSize: Int = 6) {
  catalogItemsPage(category: $category, page: $page, pageSize: $pageSize) {
    totalCount
    page
    totalPages
    hasNextPage
    items {
      id
      title
      category
      imageUrl
      price
    }
  }
}
```
ตัวอย่างตัวแปร:
```json
{
  "category": "BOOK",
  "page": 2,
  "pageSize": 4
}
```

#### การดึงข้อมูลทุกหน้า (get all)
1. ตั้ง `pageSize` เป็นค่าสูงสุดที่รองรับ (50) เพื่อให้จำนวนน้อยรอบที่สุด
2. เรียกเพจแรก (`page = 1`) แล้วใช้ค่าที่ตอบกลับ (`totalPages` หรือ `totalCount`) เพื่อคำนวณจำนวนรอบที่เหลือ
3. ไล่โหลดทีละหน้า (`page++`) จน `page > totalPages` โดยสะสม `items` ไว้ฝั่ง client

ตัวอย่าง pseudo code (JavaScript):

```javascript
const pageSize = 50;
let page = 1;
let totalPages = 1;
const allItems = [];

do {
  const { catalogItemsPage } = await graphQLClient.request(
    GET_CATALOG_PAGE,
    { page, pageSize }
  );

  allItems.push(...catalogItemsPage.items);
  totalPages = catalogItemsPage.totalPages;
  page += 1;
} while (page <= totalPages);

console.log(`รวม ${allItems.length} รายการ`);
```

#### 3. ดึงรายละเอียดสินค้า/คอร์สตามรหัส
```graphql
query GetItem($id: UUID!) {
  catalogItemById(id: $id) {
    id
    title
    description
    category
    durationMinutes
    durationHours
    imageUrl
    price
  }
}
```

### Mutation ตัวอย่าง
#### 1. เพิ่มข้อมูลใหม่
```graphql
mutation CreateItem($input: CreateCatalogItemInput!) {
  createCatalogItem(input: $input) {
    id
    title
    category
    imageUrl
    price
  }
}
```
ตัวอย่างตัวแปร:
```json
{
  "input": {
    "title": "GraphQL Hands-on Lab",
    "description": "Lab 3 ชั่วโมงสำหรับทีม Front-end",
    "category": "COURSE",
    "price": 159.99,
    "durationMinutes": 180,
    "imageUrl": "https://placehold.co/600x400?text=GraphQL+Lab"
  }
}
```

#### 2. แก้ไขข้อมูล
```graphql
mutation UpdateItem($id: UUID!, $input: UpdateCatalogItemInput!) {
  updateCatalogItem(id: $id, input: $input) {
    id
    title
    category
    imageUrl
    price
  }
}
```
ตัวอย่างตัวแปร:
```json
{
  "id": "<PUT-ID-HERE>",
  "input": {
    "price": 149.00,
    "description": "อัปเดตคำอธิบาย",
    "imageUrl": "https://placehold.co/600x400?text=GraphQL+Lab+v2"
  }
}
```

#### 3. ลบข้อมูล
```graphql
mutation DeleteItem($id: UUID!) {
  deleteCatalogItem(id: $id) {
    success
  }
}
```

## แนวทางสำหรับใช้สอน/Workshop
- เริ่มจากการให้ผู้เรียนทดลอง Query ข้อมูลทั้งหมด -> ใส่ตัวกรองด้วย variables
- ให้ทดลอง Query รายการเดี่ยวโดยส่ง `id`
- ทดสอบ Mutation ทีละตัว พร้อมให้ดู response ที่เปลี่ยนไปและผลต่อ state ฝั่ง UI
- อธิบายเพิ่มเติมเรื่อง optimistic update / refetch หลัง mutation
- สามารถขยายเพิ่มเติมด้วยการต่อ GraphQL Client เช่น Apollo, urql หรือ Relay ในฝั่ง Front-end

## การต่อยอด
- แทนที่ in-memory repository ด้วย database จริง (EF Core, Dapper)
- เพิ่มระบบ authentication/authorization ในแต่ละ Mutation
- แยกประเภทสินค้าเพิ่มเติมหรือเพิ่ม field ใหม่ เช่น stock, publishedAt, instructor เป็นต้น

## Troubleshooting & Notes
- ถ้าพอร์ต 5000 หรือ 8081 ไม่ว่าง ให้ตั้งค่า `ASPNETCORE_URLS=http://localhost:5050` หรือแก้ `docker-compose.yml` ฝั่ง host port แล้วรัน `docker compose up --build` อีกครั้ง
- Compose ใช้ volume ชื่อ `pgdata` เก็บข้อมูลถาวร หากต้องการเริ่มข้อมูลใหม่ให้ใช้ `docker compose down -v`
- Repository ทั้งสองแบบ seed ข้อมูลชุดเดียวกัน ทำให้การสาธิตมีข้อมูลตรงกันไม่ว่าจะรันแบบไหน

หวังว่าโปรเจกต์นี้จะช่วยให้การสอน GraphQL กับทีม Front-end เป็นเรื่องง่ายและสนุกมากขึ้น!
