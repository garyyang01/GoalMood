# 研究文件：團隊每日目標與心情追蹤器

**功能**: 001-goal-mood-tracker
**日期**: 2025-11-19
**目的**: 解決技術背景中的未知項目，並為設計階段提供決策依據

## 研究摘要

本文件記錄技術選型和架構決策的研究結果。由於專案遵循 constitution 中的技術棧強制執行原則，大部分技術選擇已預先確定（.NET 8, Dapper, SQLite, Vue 3, TypeScript, DaisyUI）。本研究聚焦於：

1. Dapper 最佳實踐模式
2. SQLite 資料庫設計考量
3. Vue 3 Composition API 組織模式
4. DaisyUI 組件使用指南
5. 測試策略

---

## R1: Dapper ORM 最佳實踐

### 決策

採用 Repository 模式搭配 Dapper 進行所有資料存取操作。

### 理由

1. **職責分離**: Repository 模式將資料存取邏輯與商業邏輯分離
2. **可測試性**: 可以輕鬆 mock repositories 進行單元測試
3. **Constitution 合規**: 符合 Dapper-first 原則，使用原始 SQL 查詢
4. **效能**: Dapper 提供接近原始 ADO.NET 的效能（比 EF Core 快 2-3 倍）

### 實作模式

```csharp
// Repository 介面
public interface IGoalRepository
{
    Task<IEnumerable<Goal>> GetTodayGoalsAsync(DateTime date);
    Task<Goal?> GetByIdAsync(int id);
    Task<int> CreateAsync(Goal goal);
    Task<bool> UpdateAsync(Goal goal);
    Task<bool> DeleteAsync(int id);
}

// Repository 實作
public class GoalRepository : IGoalRepository
{
    private readonly IDbConnection _db;

    public GoalRepository(IDbConnection db) => _db = db;

    public async Task<IEnumerable<Goal>> GetTodayGoalsAsync(DateTime date)
    {
        var sql = @"
            SELECT * FROM Goals
            WHERE DATE(CreatedDate) = DATE(@Date)
            ORDER BY Id";
        return await _db.QueryAsync<Goal>(sql, new { Date = date });
    }

    // ... 其他方法
}
```

### 考慮的替代方案

- **直接使用 Dapper 在 endpoints**: 被拒絕，因為違反單一職責原則且難以測試
- **Service 層**: 保留為可選，僅在商業邏輯複雜時使用（目前不需要）

---

## R2: SQLite 資料庫設計

### 決策

使用 SQLite 作為本地檔案資料庫，啟用 WAL 模式以提升併發效能。

### 理由

1. **零設定**: 無需額外安裝或設定資料庫伺服器
2. **輕量級**: 適合小型團隊（≤10 人）的規模
3. **效能**: 讀取效能優異，本地存取延遲 <1ms
4. **Constitution 要求**: 明確指定使用 SQLite

### 設定

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=GoalMood.db;Mode=ReadWriteCreate;Cache=Shared"
  }
}
```

```csharp
// Program.cs - 啟用 WAL 模式
services.AddScoped<IDbConnection>(sp =>
{
    var conn = new SqliteConnection(connectionString);
    conn.Open();
    // 啟用 Write-Ahead Logging 以提升併發效能
    new SqliteCommand("PRAGMA journal_mode=WAL;", conn).ExecuteNonQuery();
    return conn;
});
```

### Schema 設計原則

1. **簡單主鍵**: 使用 INTEGER PRIMARY KEY AUTOINCREMENT
2. **外鍵約束**: 啟用 PRAGMA foreign_keys = ON
3. **索引策略**: 在 CreatedDate 欄位建立索引以加速「今日目標」查詢
4. **資料型別**: SQLite 動態型別，但在應用層強制型別安全

### 考慮的替代方案

- **SQL Server**: 被拒絕，設定複雜度高，超出 45 分鐘目標
- **PostgreSQL**: 被拒絕，同樣設定複雜
- **In-memory**: 被拒絕，資料無法持久化

---

## R3: Vue 3 Composition API 組織模式

### 決策

採用 Composables 模式組織可重用邏輯，每個 composable 對應一個資料實體或功能領域。

### 理由

1. **程式碼重用**: Composables 允許在多個組件間共享邏輯
2. **關注點分離**: API 呼叫、狀態管理、副作用處理集中管理
3. **型別安全**: TypeScript 提供完整的型別推斷
4. **Constitution 合規**: 嚴格遵循 Composition API（禁止 Options API）

### 實作模式

```typescript
// composables/useGoals.ts
import { ref, Ref } from 'vue'
import { Goal } from '@/types/Goal'
import { apiClient } from '@/api/client'

export function useGoals() {
  const goals: Ref<Goal[]> = ref([])
  const loading: Ref<boolean> = ref(false)
  const error: Ref<string | null> = ref(null)

  const fetchTodayGoals = async () => {
    loading.value = true
    error.value = null
    try {
      const response = await apiClient.get('/api/goals/today')
      goals.value = response.data
    } catch (e) {
      error.value = '無法載入目標資料'
    } finally {
      loading.value = false
    }
  }

  const createGoal = async (memberId: number, description: string) => {
    // ...
  }

  const toggleComplete = async (goalId: number) => {
    // ...
  }

  const deleteGoal = async (goalId: number) => {
    // ...
  }

  return {
    goals,
    loading,
    error,
    fetchTodayGoals,
    createGoal,
    toggleComplete,
    deleteGoal
  }
}
```

### 組件組織

- **MemberCard.vue**: 顯示單一團隊成員的卡片（名稱、心情、目標列表）
- **GoalInput.vue**: 新增目標表單
- **MoodSelector.vue**: 更新心情表單
- **StatsPanel.vue**: 團隊統計面板

### 考慮的替代方案

- **Vuex/Pinia**: 被拒絕，對於小型應用過於複雜（constitution 明確要求僅使用 Vue reactivity）
- **Options API**: 被禁止（constitution 要求）

---

## R4: DaisyUI 組件使用指南

### 決策

使用 DaisyUI 預設主題和語意類別，避免自訂 Tailwind 配置。

### 理由

1. **一致性**: DaisyUI 提供統一的設計系統
2. **無障礙**: 內建 ARIA 屬性和鍵盤導航
3. **生產力**: 減少 CSS 撰寫時間
4. **Constitution 合規**: 要求使用 DaisyUI 組件類別

### 核心組件對應

| 功能 | DaisyUI 組件 | 類別 |
|------|-------------|------|
| 團隊成員卡片 | Card | `card`, `card-body`, `card-title` |
| 新增目標按鈕 | Button | `btn`, `btn-primary` |
| 目標勾選框 | Checkbox | `checkbox` |
| 團隊成員選擇 | Dropdown/Select | `select`, `select-bordered` |
| 心情表情符號 | Radio | `btn`, `btn-group` |
| 統計徽章 | Badge | `badge`, `badge-primary` |
| 確認對話框 | Modal | `modal`, `modal-box` |

### 範例實作

```vue
<!-- MemberCard.vue -->
<template>
  <div class="card bg-base-100 shadow-xl">
    <div class="card-body">
      <h2 class="card-title">{{ member.name }}</h2>
      <div class="badge badge-lg">{{ moodEmoji }}</div>

      <div class="space-y-2">
        <div v-for="goal in member.goals" :key="goal.id" class="flex items-center gap-2">
          <input
            type="checkbox"
            class="checkbox"
            :checked="goal.isCompleted"
            @change="toggleGoal(goal.id)"
          />
          <span :class="{ 'line-through': goal.isCompleted }">
            {{ goal.description }}
          </span>
        </div>
      </div>

      <div class="card-actions justify-end">
        <div class="badge">{{ completedCount }}/{{ totalCount }} 完成</div>
      </div>
    </div>
  </div>
</template>
```

### 考慮的替代方案

- **自訂 Tailwind 樣式**: 被拒絕，增加開發時間且違反 constitution
- **其他 UI 框架**: 被禁止（constitution 指定 DaisyUI）

---

## R5: 測試策略

### 決策

實作合約測試和整合測試，單元測試為可選。

### 理由

1. **Constitution 要求**: 條件式 TDD - 合約測試必須，單元測試可選
2. **時間效率**: 45 分鐘目標要求聚焦高價值測試
3. **風險導向**: 合約測試驗證 API 行為，整合測試驗證使用者旅程

### 測試層級

#### 1. 合約測試（必要）

測試 API endpoints 的行為，確保符合合約規格。

```csharp
// tests/contract/GoalEndpointsTests.cs
public class GoalEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task POST_Goals_ValidInput_Returns201Created()
    {
        // Arrange
        var request = new { teamMemberId = 1, description = "Test goal" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/goals", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var goal = await response.Content.ReadFromJsonAsync<GoalDto>();
        goal.Description.Should().Be("Test goal");
    }

    [Fact]
    public async Task POST_Goals_EmptyDescription_Returns400BadRequest()
    {
        // ...
    }
}
```

#### 2. 整合測試（必要）

測試端到端使用者旅程，對應規格中的驗收情境。

```csharp
// tests/integration/UserStory1_Dashboard_Tests.cs
public class UserStory1_Dashboard_Tests
{
    [Fact]
    public async Task ViewDashboard_ShowsAllTeamMembers()
    {
        // Given: 開啟應用程式
        // When: 使用者檢視儀表板
        // Then: 所有團隊成員以個別卡片形式顯示
    }
}
```

#### 3. 單元測試（可選）

僅在複雜商業邏輯需要時撰寫。

### 測試工具

- **後端**: xUnit + FluentAssertions + WebApplicationFactory
- **前端**: Vitest + Vue Test Utils（如時間允許）

### 考慮的替代方案

- **E2E 測試（Playwright）**: 被延後，超出 MVP 範圍
- **TDD-first 全覆蓋**: 被拒絕，違反 45 分鐘目標

---

## 技術風險評估

### 高風險項目

無高風險項目。所有技術選型皆為成熟技術且已在 constitution 中明確定義。

### 中風險項目

1. **Dapper 學習曲線**: 團隊可能不熟悉 Dapper
   - **緩解**: 提供範例程式碼和 repository 模板

2. **Vue 3 Composition API 新手**: 團隊可能習慣 Options API
   - **緩解**: 提供 composables 模式範例

### 低風險項目

1. **SQLite 效能**: 小規模（≤10 人）無效能問題
2. **DaisyUI 學習**: 文件完整，學習曲線低

---

## 決策日誌

| ID | 決策 | 日期 | 狀態 |
|----|------|------|------|
| D1 | 使用 Repository 模式搭配 Dapper | 2025-11-19 | ✅ 已確認 |
| D2 | SQLite WAL 模式 | 2025-11-19 | ✅ 已確認 |
| D3 | Composables 模式組織 Vue 邏輯 | 2025-11-19 | ✅ 已確認 |
| D4 | DaisyUI 預設主題 | 2025-11-19 | ✅ 已確認 |
| D5 | 合約測試優先策略 | 2025-11-19 | ✅ 已確認 |

---

## 結論

所有技術決策已完成且符合 constitution 要求。無未解決的技術未知項目。準備進入 Phase 1 設計階段。
