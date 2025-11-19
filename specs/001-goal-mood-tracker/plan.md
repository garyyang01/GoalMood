# 實作計劃：團隊每日目標與心情追蹤器

**分支**: `001-goal-mood-tracker` | **日期**: 2025-11-19 | **規格**: [spec.md](./spec.md)
**輸入**: 功能規格來自 `/specs/001-goal-mood-tracker/spec.md`

**注意**: 此範本由 `/speckit.plan` 命令填寫。執行工作流程請參閱 `.specify/templates/commands/plan.md`。

## 摘要

建立一個最小化全端網頁應用程式，讓團隊成員可以追蹤每日目標並監控團隊士氣。核心功能包括：儀表板顯示所有團隊成員的目標和心情、新增/完成/刪除目標、更新心情、以及團隊統計面板。技術方案採用 .NET 8 Web API（使用 Dapper ORM 和 SQLite）作為後端，Vue 3 + TypeScript + DaisyUI 作為前端，專注於 MVP 快速交付（45 分鐘目標）。

## 技術背景

**語言/版本**: .NET 8 (後端), TypeScript (前端), Vue 3
**主要依賴項**: Dapper ORM, SQLite, Vue 3, DaisyUI (Tailwind CSS)
**儲存**: SQLite（本地檔案資料庫）
**測試**: 根據 constitution 條件式 TDD - 合約測試優先，單元測試可選
**目標平台**: 桌面瀏覽器（Chrome, Firefox, Safari, Edge 最近 2 個版本）
**專案類型**: Web 應用程式（前後端分離）
**效能目標**:
- API 回應時間 <100ms p95（本地 SQLite）
- 前端載入時間 <2 秒（開發伺服器）
- UI 互動回應 <16ms（60fps）

**限制**:
- 無使用者驗證
- 桌面專用（無響應式設計）
- 小型團隊規模（≤10 人）
- 單日視圖（無多日歷史查詢 UI）
- 45 分鐘實作目標

**規模/範圍**:
- 最多 10 個團隊成員
- 最多 50 個總目標
- 5 個使用者故事（P1-P5）
- 23 項功能需求

## 憲法檢查

*關卡：必須在 Phase 0 研究前通過。Phase 1 設計後重新檢查。*

### ✅ I. MVP-First Development

- **合規狀態**: PASS
- **驗證**:
  - ✅ 僅建構規格中明確要求的功能（20+4 項功能需求）
  - ✅ 採用最簡單實作（無驗證、無多日歷史、無通知）
  - ✅ 範圍外明確排除 15 項功能
  - ✅ 桌面專用介面
  - ✅ SQLite 作為資料庫

### ✅ II. Technology Stack Enforcement

- **合規狀態**: PASS
- **驗證**:
  - ✅ 後端: .NET 8 Web API + Dapper + SQLite + nullable reference types + async/await
  - ✅ 前端: Vue 3 + TypeScript strict + Composition API + DaisyUI
  - ✅ 禁止 Entity Framework（僅使用 Dapper）
  - ✅ 禁止 Options API（僅使用 Composition API）

### ⚠️ III. Test-Driven Development (CONDITIONAL)

- **合規狀態**: CONDITIONAL PASS
- **驗證**:
  - ⚠️ 規格中的驗收情境需要合約測試（API endpoints）
  - ✅ 無外部系統整合
  - ✅ 資料模型相對簡單（3 個實體）
- **測試策略**:
  - **必要**: 合約測試（API endpoints 行為驗證）
  - **可選**: 單元測試（除非複雜度證明有必要）
  - **必要**: 整合測試（端到端使用者旅程）

### ✅ IV. Code Quality Standards

- **合規狀態**: PASS
- **驗證**:
  - ✅ C# nullable reference types 啟用
  - ✅ 使用 record types 作為 DTOs
  - ✅ Minimal API endpoints（非傳統 controllers）
  - ✅ TypeScript strict mode 啟用
  - ✅ 使用 `<script setup lang="ts">` 語法
  - ✅ 組織結構: 後端 `/Models`, `/Data`, `/Services`（可選），前端 `/components`, `/composables`, `/types`, `/api`

### ✅ V. User Experience Consistency

- **合規狀態**: PASS
- **驗證**:
  - ✅ 使用 DaisyUI 語意類別（btn, card, dropdown, checkbox, badge）
  - ✅ 一致的互動模式（表單提交即時回饋、錯誤訊息、載入狀態）
  - ✅ 無障礙基準（語意 HTML、鍵盤導航、螢幕閱讀器標籤）

### ✅ VI. Performance Requirements

- **合規狀態**: PASS
- **驗證**:
  - ✅ 後端效能目標: <100ms p95（符合規格 SC-001 的 2 秒載入）
  - ✅ 無 N+1 查詢（使用 Dapper eager loading）
  - ✅ 前端效能目標: <2 秒初始載入，<16ms UI 互動
  - ✅ 規模限制明確（20 人上限、100 個目標上限）

### ✅ VII. Dapper-First Data Access

- **合規狀態**: PASS
- **驗證**:
  - ✅ 所有資料庫操作使用 Dapper
  - ✅ 原始 SQL 查詢在 repository 類別中
  - ✅ 參數化查詢（防止 SQL injection）
  - ✅ 使用 `QueryAsync<T>` 讀取，`ExecuteAsync` 寫入
  - ✅ SQL 遷移腳本在 `/Migrations` 目錄

### ✅ Data Model Constraints

- **合規狀態**: PASS
- **驗證**:
  - ✅ 核心實體符合 constitution 定義（TeamMember, Goal）
  - ✅ 一對多關係: TeamMember → Goals（cascade delete）
  - ✅ 無多對多關係
  - ✅ 無稽核表（無 created_by, updated_by）

### ✅ API Contract Standards

- **合規狀態**: PASS
- **驗證**:
  - ✅ RESTful endpoint 命名
  - ✅ 標準 HTTP 狀態碼（200/201 成功，400 驗證錯誤，404 找不到，500 伺服器錯誤）
  - ✅ JSON 回應格式
  - ✅ 合約文件在 `/specs/001-goal-mood-tracker/contracts/`

### ✅ Development Workflow

- **合規狀態**: PASS
- **驗證**:
  - ✅ Spec-driven process（specify → plan → tasks → implement → analyze）
  - ✅ 使用者故事獨立性（P1-P5 優先級）
  - ✅ Git workflow（feature branch `001-goal-mood-tracker`）

### 📋 憲法檢查摘要

**總體狀態**: ✅ PASS（所有關卡通過，一個條件式通過）

**無違規需要證明**

## 專案結構

### 文件（此功能）

```text
specs/001-goal-mood-tracker/
├── spec.md              # 功能規格（/speckit.specify 輸出）
├── plan.md              # 本檔案（/speckit.plan 輸出）
├── research.md          # Phase 0 輸出（/speckit.plan 生成）
├── data-model.md        # Phase 1 輸出（/speckit.plan 生成）
├── quickstart.md        # Phase 1 輸出（/speckit.plan 生成）
├── contracts/           # Phase 1 輸出（/speckit.plan 生成）
│   └── api.yaml        # OpenAPI 3.0 規格
└── tasks.md             # Phase 2 輸出（/speckit.tasks - 非本命令建立）
```

### 原始碼（repository root）

```text
GoalMood.BE/                    # .NET 8 Web API 專案
├── Models/                     # 領域模型和 DTOs
│   ├── TeamMember.cs          # 團隊成員實體
│   ├── Goal.cs                # 目標實體
│   ├── Mood.cs                # 心情 enum
│   └── DTOs/                  # 資料傳輸物件
│       ├── GoalDto.cs
│       ├── TeamMemberDto.cs
│       └── StatsDto.cs
├── Data/                       # Dapper repositories
│   ├── ITeamMemberRepository.cs
│   ├── TeamMemberRepository.cs
│   ├── IGoalRepository.cs
│   ├── GoalRepository.cs
│   └── DbContext.cs           # SQLite 連線管理
├── Services/                   # 商業邏輯（可選，保持簡單）
│   ├── IGoalService.cs
│   └── GoalService.cs
├── Endpoints/                  # Minimal API endpoints
│   ├── TeamMemberEndpoints.cs
│   ├── GoalEndpoints.cs
│   └── StatsEndpoints.cs
├── Migrations/                 # SQL 遷移腳本
│   └── 001_InitialSchema.sql
├── Program.cs                  # 應用程式進入點
├── appsettings.json           # 設定（含 SQLite 連線字串）
└── GoalMood.BE.csproj

GoalMood.FE/                    # Vue 3 + TypeScript 專案
├── src/
│   ├── components/            # Vue 組件
│   │   ├── MemberCard.vue    # 團隊成員卡片
│   │   ├── GoalInput.vue     # 新增目標表單
│   │   ├── MoodSelector.vue  # 更新心情表單
│   │   └── StatsPanel.vue    # 統計面板
│   ├── composables/           # Composition API 可重用邏輯
│   │   ├── useGoals.ts       # 目標 CRUD 邏輯
│   │   ├── useMoods.ts       # 心情更新邏輯
│   │   └── useStats.ts       # 統計資料邏輯
│   ├── types/                 # TypeScript 型別定義
│   │   ├── TeamMember.ts
│   │   ├── Goal.ts
│   │   └── Stats.ts
│   ├── api/                   # API 客戶端
│   │   └── client.ts         # HTTP 請求封裝
│   ├── App.vue                # 根組件
│   └── main.ts                # 應用程式進入點
├── public/
├── index.html
├── package.json
├── tsconfig.json              # TypeScript strict mode 設定
├── tailwind.config.js         # Tailwind + DaisyUI 設定
└── vite.config.ts

tests/                          # 測試目錄
├── contract/                   # API 合約測試
│   ├── TeamMemberEndpointsTests.cs
│   ├── GoalEndpointsTests.cs
│   └── StatsEndpointsTests.cs
├── integration/                # 端到端測試
│   ├── UserStory1_Dashboard_Tests.cs
│   ├── UserStory2_AddGoal_Tests.cs
│   └── UserStory3_UpdateMood_Tests.cs
└── unit/                       # 單元測試（可選）
    └── Services/
        └── GoalServiceTests.cs
```

**結構決策**:

選擇 Web 應用程式結構（Option 2）因為：
1. 規格明確要求前後端分離（前端 Vue 3，後端 .NET 8 Web API）
2. 後端已存在 `GoalMood.BE/` 專案目錄
3. 前端將建立為 `GoalMood.FE/` 獨立專案
4. 測試組織在根目錄 `tests/`，涵蓋合約、整合和可選的單元測試

## 複雜度追蹤

> **僅在憲法檢查有需要證明的違規時填寫**

無違規需要證明。所有憲法檢查項目皆通過。
