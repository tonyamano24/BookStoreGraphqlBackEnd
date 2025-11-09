# BookStore GraphQL Backend

โปรเจกต์นี้เป็นตัวอย่าง Backend สำหรับสอนการเรียกใช้งาน GraphQL API ด้วย .NET 8 (C#) สำหรับเว็บ/แอปที่ต้องการจัดการ "รายการหนังสือ-คอร์ส-สินค้า" โดยเน้นให้ฝั่ง Front-end ได้ฝึกฝนการเขียน Query, Mutation, การจัดการ state, loader และ error handling

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
2. ติดตั้ง PostgreSQL (ทดสอบแล้วกับเวอร์ชัน 15+) หรือใช้ Docker ตามตัวอย่างด้านล่าง
3. ตั้งค่า connection string ใน `appsettings.json` หรือ environment variable `POSTGRES_CONNECTION_STRING` เช่น  
   `Host=localhost;Port=5432;Database=bookstore;Username=bookstore_app;Password=change-me`
4. เปิดเทอร์มินัลที่โฟลเดอร์โปรเจกต์นี้แล้วรัน
   ```bash
   dotnet restore ./src/BookStore.Api/BookStore.Api.csproj
   dotnet run --project ./src/BookStore.Api/BookStore.Api.csproj
   ```
5. เซิร์ฟเวอร์จะเปิดที่ `http://localhost:5000` (หรือพอร์ตที่ .NET กำหนด) และ GraphQL Playground/Schema explorer สามารถเข้าผ่าน `http://localhost:5000/graphql`

> **หมายเหตุ:** ถ้าไม่ตั้งค่า connection string ระบบจะ fallback กลับไปใช้ in-memory repository เหมือนเดิม เหมาะกับ workshop ที่ไม่ต้องมีฐานข้อมูลจริง

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
2. รอ healthcheck ของ Postgres แล้วเข้า `http://localhost:8080/graphql`
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
  - `createdAtUtc: DateTime!`
  - **ฟิลด์เสริมจาก type extension**
    - `categoryDisplayName: String!`
    - `durationHours: Float!`
- `CatalogItemCategory` (enum): `BOOK`, `COURSE`, `MERCHANDISE`

### Query ตัวอย่าง
#### 1. ดึงรายการทั้งหมด (พร้อมตัวกรอง category)
```graphql
query GetCatalog($category: CatalogItemCategory) {
  catalogItems(category: $category) {
    id
    title
    category
    categoryDisplayName
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

#### 2. ดึงรายละเอียดสินค้า/คอร์สตามรหัส
```graphql
query GetItem($id: UUID!) {
  catalogItemById(id: $id) {
    id
    title
    description
    category
    durationMinutes
    durationHours
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
    "durationMinutes": 180
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
    "description": "อัปเดตคำอธิบาย"
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

หวังว่าโปรเจกต์นี้จะช่วยให้การสอน GraphQL กับทีม Front-end เป็นเรื่องง่ายและสนุกมากขึ้น!
