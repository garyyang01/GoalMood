# 資料模型：團隊每日目標與心情追蹤器

**功能**: 001-goal-mood-tracker
**日期**: 2025-11-19
**基於**: [spec.md](./spec.md)

## 摘要

本文件定義應用程式的資料結構，包括實體、關聯、驗證規則和資料庫 schema。設計遵循 constitution 中的資料模型約束：簡單關聯、無稽核欄位、使用 Dapper 進行資料存取。

---

## 實體模型

### 1. TeamMember（團隊成員）

**用途**: 代表團隊中的個人

**屬性**:

| 欄位名稱 | 型別 | 限制 | 說明 |
|---------|------|------|------|
| Id | int | PK, AUTO_INCREMENT | 唯一識別碼 |
| Name | string(50) | NOT NULL | 成員姓名 |
| CurrentMood | Mood (enum) | NOT NULL, DEFAULT 'Neutral' | 當前心情狀態 |

**驗證規則**:
- Name: 非空白，長度 1-50 字元（FR-024）
- CurrentMood: 必須為五種心情之一（Happy, Content, Neutral, Sad, Stressed）

**關聯**:
- `Goals`: 一對多（一個成員有多個目標）

**C# 模型**:

```csharp
public class TeamMember
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Mood CurrentMood { get; set; } = Mood.Neutral;

    // Navigation property (not mapped to DB in Dapper)
    public List<Goal>? Goals { get; set; }
}
```

---

### 2. Goal（目標）

**用途**: 代表分配給團隊成員的每日任務或目標

**屬性**:

| 欄位名稱 | 型別 | 限制 | 說明 |
|---------|------|------|------|
| Id | int | PK, AUTO_INCREMENT | 唯一識別碼 |
| TeamMemberId | int | FK → TeamMember.Id, NOT NULL | 所屬團隊成員 |
| Description | string(500) | NOT NULL | 目標描述 |
| IsCompleted | bool | NOT NULL, DEFAULT FALSE | 完成狀態 |
| CreatedDate | DateTime | NOT NULL, DEFAULT CURRENT_TIMESTAMP | 建立日期 |

**驗證規則**:
- Description: 非空白，長度 1-500 字元（FR-014）
- CreatedDate: 預設為今天（FR-018）
- TeamMemberId: 必須參照存在的 TeamMember（FK constraint）

**關聯**:
- `TeamMember`: 多對一（多個目標屬於一個成員）

**C# 模型**:

```csharp
public class Goal
{
    public int Id { get; set; }
    public int TeamMemberId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Navigation property (not mapped to DB in Dapper)
    public TeamMember? TeamMember { get; set; }
}
```

---

### 3. Mood（心情枚舉）

**用途**: 定義五種可能的心情狀態

**C# 枚舉**:

```csharp
public enum Mood
{
    Happy = 1,      // 😀 開心
    Content = 2,    // 😊 滿足
    Neutral = 3,    // 😐 中性
    Sad = 4,        // 😞 悲傷
    Stressed = 5    // 😤 壓力
}
```

**表情符號對應**:

| 枚舉值 | 表情符號 | 中文描述 |
|--------|---------|---------|
| Happy | 😀 | 開心 |
| Content | 😊 | 滿足 |
| Neutral | 😐 | 中性 |
| Sad | 😞 | 悲傷 |
| Stressed | 😤 | 壓力 |

---

## 資料庫 Schema（SQLite）

### 建立語句

```sql
-- 團隊成員表
CREATE TABLE TeamMembers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL CHECK(LENGTH(Name) > 0 AND LENGTH(Name) <= 50),
    CurrentMood INTEGER NOT NULL DEFAULT 3 CHECK(CurrentMood BETWEEN 1 AND 5)
);

-- 目標表
CREATE TABLE Goals (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TeamMemberId INTEGER NOT NULL,
    Description TEXT NOT NULL CHECK(LENGTH(Description) > 0 AND LENGTH(Description) <= 500),
    IsCompleted INTEGER NOT NULL DEFAULT 0 CHECK(IsCompleted IN (0, 1)),
    CreatedDate TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (TeamMemberId) REFERENCES TeamMembers(Id) ON DELETE CASCADE
);

-- 索引：加速「今日目標」查詢（FR-021）
CREATE INDEX idx_goals_created_date ON Goals(CreatedDate);
CREATE INDEX idx_goals_team_member ON Goals(TeamMemberId);
```

### 啟用外鍵約束

```sql
PRAGMA foreign_keys = ON;
```

### 啟用 WAL 模式（提升併發效能）

```sql
PRAGMA journal_mode = WAL;
```

---

## 關聯圖

```text
┌──────────────────┐
│   TeamMember     │
├──────────────────┤
│ Id (PK)          │
│ Name             │
│ CurrentMood      │
└────────┬─────────┘
         │
         │ 1:N
         │
         ▼
┌──────────────────┐
│      Goal        │
├──────────────────┤
│ Id (PK)          │
│ TeamMemberId (FK)│
│ Description      │
│ IsCompleted      │
│ CreatedDate      │
└──────────────────┘
```

**關聯規則**:
- 一個 TeamMember 可以有多個 Goals（一對多）
- 刪除 TeamMember 時，其所有 Goals 也會被刪除（CASCADE DELETE）
- Goal 必須屬於一個存在的 TeamMember（FK constraint）

---

## DTO（資料傳輸物件）

### GoalDto

**用途**: API 回應中的目標資料

```csharp
public record GoalDto(
    int Id,
    int TeamMemberId,
    string Description,
    bool IsCompleted,
    DateTime CreatedDate
);
```

### TeamMemberDto

**用途**: API 回應中的團隊成員資料（包含目標）

```csharp
public record TeamMemberDto(
    int Id,
    string Name,
    Mood CurrentMood,
    string MoodEmoji,  // 計算屬性
    List<GoalDto> Goals,
    int CompletedCount,  // 計算屬性
    int TotalCount       // 計算屬性
);
```

### StatsDto

**用途**: 團隊統計資料

```csharp
public record StatsDto(
    double CompletionPercentage,   // 團隊完成百分比
    Dictionary<Mood, int> MoodDistribution  // 心情分布
);
```

### CreateGoalRequest

**用途**: 新增目標的請求

```csharp
public record CreateGoalRequest(
    int TeamMemberId,
    string Description
);
```

### UpdateMoodRequest

**用途**: 更新心情的請求

```csharp
public record UpdateMoodRequest(
    Mood Mood
);
```

---

## 狀態轉換

### Goal 狀態轉換

```text
[建立]
  │
  ▼
[IsCompleted = false] ◄─────┐
  │                         │
  │ 勾選核取方塊             │ 取消勾選核取方塊
  ▼                         │
[IsCompleted = true] ────────┘
  │
  │ 刪除（需確認）
  ▼
[已刪除]
```

**轉換規則**:
- 新建立的 Goal 預設為 IsCompleted = false（FR-002）
- 使用者可以隨時切換 IsCompleted 狀態（FR-006, FR-007）
- 刪除需要確認對話框（澄清事項 #5）

### TeamMember CurrentMood 轉換

```text
[任何心情狀態]
  │
  │ 使用者選擇新心情
  ▼
[新心情狀態]
```

**轉換規則**:
- CurrentMood 可以隨時更新為五種心情之一（FR-005）
- 不保留歷史心情（僅當前心情）
- 最後寫入勝出（澄清事項 #2）

---

## 資料完整性規則

### 參照完整性

1. **Goal.TeamMemberId**: 必須參照存在的 TeamMember.Id（FK constraint）
2. **CASCADE DELETE**: 刪除 TeamMember 時自動刪除其所有 Goals

### 值域完整性

1. **TeamMember.Name**: 1-50 字元，非空白
2. **Goal.Description**: 1-500 字元，非空白
3. **Mood**: 必須為 1-5 之間的整數（對應五種心情）
4. **IsCompleted**: 必須為 0 或 1（SQLite boolean）

### 業務規則

1. **今日目標過濾**: 儀表板僅顯示 `DATE(CreatedDate) = DATE('now', 'localtime')` 的目標（FR-021）
2. **歷史資料保留**: 所有目標永久保留，不自動刪除（澄清事項 #1）
3. **併發控制**: 最後寫入勝出，無樂觀鎖定（澄清事項 #2）

---

## 資料遷移策略

### 初始 Schema（v1.0）

檔案: `GoalMood.BE/Migrations/001_InitialSchema.sql`

```sql
-- 啟用外鍵約束
PRAGMA foreign_keys = ON;

-- 建立 TeamMembers 表
CREATE TABLE TeamMembers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL CHECK(LENGTH(Name) > 0 AND LENGTH(Name) <= 50),
    CurrentMood INTEGER NOT NULL DEFAULT 3 CHECK(CurrentMood BETWEEN 1 AND 5)
);

-- 建立 Goals 表
CREATE TABLE Goals (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TeamMemberId INTEGER NOT NULL,
    Description TEXT NOT NULL CHECK(LENGTH(Description) > 0 AND LENGTH(Description) <= 500),
    IsCompleted INTEGER NOT NULL DEFAULT 0 CHECK(IsCompleted IN (0, 1)),
    CreatedDate TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (TeamMemberId) REFERENCES TeamMembers(Id) ON DELETE CASCADE
);

-- 建立索引
CREATE INDEX idx_goals_created_date ON Goals(CreatedDate);
CREATE INDEX idx_goals_team_member ON Goals(TeamMemberId);

-- 插入範例資料（開發用）
INSERT INTO TeamMembers (Name, CurrentMood) VALUES
    ('Alice', 1),   -- Happy
    ('Bob', 3),     -- Neutral
    ('Carol', 2);   -- Content

INSERT INTO Goals (TeamMemberId, Description, IsCompleted) VALUES
    (1, '完成專案規劃', 0),
    (1, '撰寫技術文件', 1),
    (2, '進行程式碼審查', 0);
```

### 遷移執行

```csharp
// Program.cs
public static void RunMigrations(string connectionString)
{
    using var connection = new SqliteConnection(connectionString);
    connection.Open();

    var sql = File.ReadAllText("Migrations/001_InitialSchema.sql");
    connection.Execute(sql);
}
```

---

## 效能考量

### 查詢優化

1. **今日目標查詢**: 使用 `idx_goals_created_date` 索引加速
2. **成員目標查詢**: 使用 `idx_goals_team_member` 索引加速
3. **避免 N+1**: 使用 Dapper Multi-Mapping 一次載入 TeamMember 及其 Goals

### 範例優化查詢

```csharp
// 一次載入所有成員及其今日目標（避免 N+1）
public async Task<IEnumerable<TeamMember>> GetAllMembersWithTodayGoalsAsync()
{
    var sql = @"
        SELECT
            tm.Id, tm.Name, tm.CurrentMood,
            g.Id, g.TeamMemberId, g.Description, g.IsCompleted, g.CreatedDate
        FROM TeamMembers tm
        LEFT JOIN Goals g ON tm.Id = g.TeamMemberId
            AND DATE(g.CreatedDate) = DATE('now', 'localtime')
        ORDER BY tm.Id, g.Id";

    var memberDict = new Dictionary<int, TeamMember>();

    await _db.QueryAsync<TeamMember, Goal?, TeamMember>(
        sql,
        (member, goal) =>
        {
            if (!memberDict.TryGetValue(member.Id, out var existingMember))
            {
                existingMember = member;
                existingMember.Goals = new List<Goal>();
                memberDict.Add(member.Id, existingMember);
            }

            if (goal != null)
            {
                existingMember.Goals!.Add(goal);
            }

            return existingMember;
        },
        splitOn: "Id"
    );

    return memberDict.Values;
}
```

### 預期效能

- **讀取**: <10ms（本地 SQLite，有索引）
- **寫入**: <5ms（本地 SQLite）
- **規模**: 支援 10 人 x 50 目標 = 500 筆資料，遠低於 SQLite 限制

---

## 測試資料

### 種子資料（開發/測試環境）

```csharp
public static void SeedTestData(IDbConnection db)
{
    // 清空現有資料
    db.Execute("DELETE FROM Goals");
    db.Execute("DELETE FROM TeamMembers");
    db.Execute("DELETE FROM sqlite_sequence WHERE name IN ('Goals', 'TeamMembers')");

    // 插入測試團隊成員
    db.Execute(@"
        INSERT INTO TeamMembers (Name, CurrentMood) VALUES
            ('Alice Chen', 1),
            ('Bob Wang', 3),
            ('Carol Lin', 2),
            ('David Wu', 4),
            ('Eve Huang', 5)
    ");

    // 插入測試目標
    db.Execute(@"
        INSERT INTO Goals (TeamMemberId, Description, IsCompleted, CreatedDate) VALUES
            (1, '完成需求分析文件', 1, datetime('now', 'localtime')),
            (1, '與客戶開會確認規格', 0, datetime('now', 'localtime')),
            (2, '進行程式碼審查', 0, datetime('now', 'localtime')),
            (3, '撰寫單元測試', 1, datetime('now', 'localtime')),
            (3, '修復 Bug #123', 1, datetime('now', 'localtime')),
            (4, '學習新技術框架', 0, datetime('now', 'localtime'))
    ");
}
```

---

## 資料模型驗證檢查清單

- [x] 所有實體都有主鍵（Id）
- [x] 外鍵約束正確定義（Goal.TeamMemberId → TeamMember.Id）
- [x] 驗證規則對應規格需求（FR-014, FR-024）
- [x] 索引策略支援主要查詢模式（今日目標、成員目標）
- [x] DTOs 對應 API 合約
- [x] 資料型別符合 SQLite 限制
- [x] 無稽核欄位（created_by, updated_by）- Constitution 要求
- [x] CASCADE DELETE 規則明確
- [x] 預設值符合業務規則

---

## 結論

資料模型設計完成，符合所有規格需求和 constitution 約束。準備進入 API 合約設計階段。
