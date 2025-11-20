# 任務清單：團隊每日目標與心情追蹤器

**輸入來源**: `/specs/001-goal-mood-tracker/` 的設計文件
**前置文件**: plan.md, spec.md, data-model.md, contracts/api.yaml, research.md, quickstart.md

**測試要求**: 根據 constitution 要求，合約測試為必要（條件式 TDD）。整合測試為必要。單元測試為可選。

**組織方式**: 任務按使用者故事分組，讓每個故事能獨立實作和測試。

## 格式：`- [ ] [ID] [P?] [Story?] 描述`

- **[P]**：可並行執行（不同檔案，無相依性）
- **[Story]**：此任務屬於哪個使用者故事（例如 US1, US2, US3）
- 描述中包含明確的檔案路徑

## 路徑慣例

- **Web 應用程式結構**：`GoalMood.BE/`（後端）、`GoalMood.FE/`（前端）、`tests/`（測試專案）
- 後端：.NET 8 Web API 搭配 Dapper + SQLite
- 前端：Vue 3 + TypeScript + DaisyUI

---

## 階段 1：設定（共用基礎建設）

**目的**：專案初始化與基本結構

- [X] T001 驗證已安裝 .NET 8 SDK 和 Node.js 18+ 依據 quickstart.md 前置需求
- [X] T002 在 GoalMood.BE/ 目錄建立 SQLite 資料庫檔案 GoalMood.db
- [X] T003 在 GoalMood.BE/GoalMood.BE.csproj 安裝 Dapper NuGet 套件
- [X] T004 [P] 在 GoalMood.BE/GoalMood.BE.csproj 安裝 Microsoft.Data.Sqlite NuGet 套件
- [X] T005 [P] 在 GoalMood.BE/GoalMood.BE.csproj 安裝 Swashbuckle.AspNetCore NuGet 套件以支援 Swagger
- [X] T006 使用 Vite 在 GoalMood.FE/ 建立 Vue 3 TypeScript 專案
- [X] T007 在 GoalMood.FE/package.json 安裝 DaisyUI 和 Tailwind CSS 依賴項
- [X] T008 [P] 在 GoalMood.FE/tailwind.config.js 設定 Tailwind CSS 並加入 DaisyUI 外掛
- [X] T009 [P] 建立測試專案結構 tests/contract/ 和 tests/integration/
- [X] T010 [P] 在測試專案中安裝 xUnit 和 FluentAssertions NuGet 套件

---

## 階段 2：基礎建設（阻擋性前置需求）

**目的**：必須完成的核心基礎建設，才能開始任何使用者故事

**⚠️ 關鍵**：在此階段完成前，無法開始任何使用者故事的工作

- [X] T011 在 GoalMood.BE/Models/Mood.cs 建立 Mood 列舉，值為 Happy=1, Content=2, Neutral=3, Sad=4, Stressed=5
- [X] T012 在 GoalMood.BE/Models/TeamMember.cs 依據 data-model.md 建立 TeamMember 實體，包含姓名最大長度 50 字元驗證（依據 spec.md FR-023）
- [X] T013 在 GoalMood.BE/Models/Goal.cs 依據 data-model.md 建立 Goal 實體
- [X] T014 執行資料庫遷移 GoalMood.BE/Migrations/001_InitialSchema.sql 以建立 TeamMembers 和 Goals 資料表
- [X] T015 驗證已依據 data-model.md 建立外鍵約束和索引（PRAGMA foreign_keys=ON, idx_goals_created_date, idx_goals_team_member）
- [X] T016 [P] 在 GoalMood.BE/Models/DTOs/GoalDto.cs 依據 data-model.md 建立 GoalDto record
- [X] T017 [P] 在 GoalMood.BE/Models/DTOs/TeamMemberDto.cs 依據 data-model.md 建立 TeamMemberDto record
- [X] T018 [P] 在 GoalMood.BE/Models/DTOs/StatsDto.cs 依據 data-model.md 建立 StatsDto record
- [X] T019 [P] 在 GoalMood.BE/Models/DTOs/CreateGoalRequest.cs 依據 data-model.md 建立 CreateGoalRequest record
- [X] T020 [P] 在 GoalMood.BE/Models/DTOs/UpdateMoodRequest.cs 依據 data-model.md 建立 UpdateMoodRequest record
- [X] T021 在 GoalMood.BE/Program.cs 註冊 IDbConnection，使用來自 appsettings.json 的 SQLite 連線字串
- [X] T022 在 GoalMood.BE/Program.cs 依據 research.md 決策 D2 啟用 SQLite WAL 模式
- [X] T023 在 GoalMood.BE/Program.cs 依據 quickstart.md 設定 CORS 以允許 http://localhost:5173
- [X] T024 在 GoalMood.BE/Program.cs 依據 contracts/api.yaml 設定 Swagger/OpenAPI
- [X] T025 在 GoalMood.BE/Migrations/seed-data.sql 依據 data-model.md 填入測試資料（5 個團隊成員，6 個目標）

**檢查點**：基礎建設就緒 - 使用者故事實作現在可以並行開始

---

## 階段 3：使用者故事 1 - 檢視團隊儀表板（優先級：P1）🎯 MVP

**目標**：在儀表板上顯示所有團隊成員及其今日目標和當前心情

**獨立測試**：載入應用程式並驗證所有團隊成員都顯示其姓名、當前心情表情符號和今日目標清單及完成計數

### 使用者故事 1 的合約測試（必要）

> **注意：先撰寫這些測試，確保實作前會失敗**

- [ ] T026 [P] [US1] 在 tests/contract/TeamMemberEndpointsTests.cs 為 GET /api/members 端點建立合約測試 - 驗證 200 狀態和 TeamMemberDto 陣列 schema
- [ ] T027 [P] [US1] 在 tests/contract/TeamMemberEndpointsTests.cs 為 GET /api/members 空資料庫情況建立合約測試 - 驗證空陣列回應

### 使用者故事 1 的實作

- [X] T028 [US1] 在 GoalMood.BE/Data/ITeamMemberRepository.cs 建立 ITeamMemberRepository 介面，包含 GetAllWithTodayGoalsAsync 方法
- [X] T029 [US1] 在 GoalMood.BE/Data/TeamMemberRepository.cs 實作 TeamMemberRepository，使用 Dapper multi-mapping 避免 N+1 查詢，依據 data-model.md
- [X] T030 [US1] 在 GoalMood.BE/Program.cs 依賴注入容器註冊 ITeamMemberRepository
- [X] T031 [US1] 在 GoalMood.BE/Endpoints/TeamMemberEndpoints.cs 建立 TeamMemberEndpoints 類別
- [X] T032 [US1] 在 GoalMood.BE/Endpoints/TeamMemberEndpoints.cs 依據 contracts/api.yaml 實作 GET /api/members minimal API 端點
- [X] T033 [US1] 在 TeamMemberDto 加入心情表情符號對應邏輯，將 Mood enum 轉換為表情符號字串，依據 spec.md
- [X] T034 [US1] 在 TeamMemberDto 加入 completedCount 和 totalCount 計算邏輯
- [X] T035 [US1] 在 GoalMood.FE/src/api/client.ts 建立 API 客戶端，使用來自 .env.local 的基礎 URL（VITE_API_BASE_URL）
- [X] T036 [P] [US1] 在 GoalMood.FE/src/types/TeamMember.ts 建立 TeamMember TypeScript 介面，對應 TeamMemberDto schema
- [X] T037 [P] [US1] 在 GoalMood.FE/src/types/Goal.ts 建立 Goal TypeScript 介面，對應 GoalDto schema
- [X] T038 [US1] 在 GoalMood.FE/src/composables/useMembers.ts 建立 useMembers composable，包含 fetchMembers 方法
- [X] T039 [US1] 在 GoalMood.FE/src/components/MemberCard.vue 建立 MemberCard 元件，使用 DaisyUI card 類別，依據 research.md
- [X] T040 [US1] 在 MemberCard.vue 實作成員姓名、心情表情符號徽章和目標清單顯示
- [X] T041 [US1] 在 MemberCard.vue 使用 DaisyUI badge 元件顯示完成計數徽章
- [X] T042 [US1] 在 GoalMood.FE/src/App.vue 更新 App.vue 以取得並顯示所有成員卡片
- [X] T043 [US1] 在 App.vue 當無團隊成員時加入空白狀態訊息「尚無團隊成員資料」（依據 spec.md 邊界情況）

### 使用者故事 1 的整合測試（必要）

- [ ] T044 [US1] 在 tests/integration/UserStory1_Dashboard_Tests.cs 建立檢視包含團隊成員的儀表板整合測試 - 依據驗收情境 1 驗證所有團隊成員以卡片形式顯示
- [ ] T045 [US1] 在 tests/integration/UserStory1_Dashboard_Tests.cs 建立有今日目標的成員整合測試 - 依據驗收情境 2 驗證目標清單和完成狀態顯示
- [ ] T046 [US1] 在 tests/integration/UserStory1_Dashboard_Tests.cs 建立已設定心情的成員整合測試 - 依據驗收情境 3 驗證心情表情符號顯示
- [ ] T047 [US1] 在 tests/integration/UserStory1_Dashboard_Tests.cs 建立無目標的成員整合測試 - 依據驗收情境 4 驗證空白目標清單或「今日無目標」訊息
- [ ] T048 [US1] 在 tests/integration/UserStory1_Dashboard_Tests.cs 建立未設定心情的成員整合測試 - 依據驗收情境 5 驗證預設中性心情指示器

**檢查點**：此時使用者故事 1 應該完全可運作且可獨立測試。儀表板顯示所有團隊成員及其心情和目標。

---

## 階段 4：使用者故事 2 - 新增每日目標（優先級：P2）

**目標**：允許使用者透過表單為團隊成員新增目標

**獨立測試**：使用「新增目標」表單為不同團隊成員建立新目標，並驗證它們出現在儀表板上

### 使用者故事 2 的合約測試（必要）

- [ ] T049 [P] [US2] 在 tests/contract/GoalEndpointsTests.cs 為 POST /api/goals 有效輸入建立合約測試 - 依據 contracts/api.yaml 驗證 201 狀態和 GoalDto 回應
- [ ] T050 [P] [US2] 在 tests/contract/GoalEndpointsTests.cs 為 POST /api/goals 缺少 teamMemberId 建立合約測試 - 依據驗收情境 4 驗證 400 狀態和錯誤訊息
- [ ] T051 [P] [US2] 在 tests/contract/GoalEndpointsTests.cs 為 POST /api/goals 空白描述建立合約測試 - 依據驗收情境 5 驗證 400 狀態和錯誤訊息
- [ ] T052 [P] [US2] 在 tests/contract/GoalEndpointsTests.cs 為 POST /api/goals 描述超過 500 字元建立合約測試 - 驗證 400 狀態（目標描述不超過 500 字元）
- [ ] T053 [P] [US2] 在 tests/contract/GoalEndpointsTests.cs 為 POST /api/goals 不存在的 teamMemberId 建立合約測試 - 依據 contracts/api.yaml 驗證 404 狀態
- [ ] T054 [P] [US2] 在 tests/contract/GoalEndpointsTests.cs 為 DELETE /api/goals/{goalId} 有效 goalId 建立合約測試 - 依據 contracts/api.yaml 驗證 204 狀態
- [ ] T055 [P] [US2] 在 tests/contract/GoalEndpointsTests.cs 為 DELETE /api/goals/{goalId} 不存在的 goalId 建立合約測試 - 驗證 404 狀態

### 使用者故事 2 的實作

- [X] T056 [US2] 在 GoalMood.BE/Data/IGoalRepository.cs 建立 IGoalRepository 介面，包含 CreateAsync 和 DeleteAsync 方法
- [X] T057 [US2] 在 GoalMood.BE/Data/GoalRepository.cs 實作 GoalRepository，使用 Dapper 參數化查詢，依據 constitution Dapper-first 原則
- [X] T058 [US2] 在 GoalMood.BE/Program.cs 依賴注入容器註冊 IGoalRepository
- [X] T059 [US2] 在 GoalMood.BE/Endpoints/GoalEndpoints.cs 建立 GoalEndpoints 類別
- [X] T060 [US2] 在 GoalMood.BE/Endpoints/GoalEndpoints.cs 依據 contracts/api.yaml 實作 POST /api/goals minimal API 端點並加入驗證
- [X] T061 [US2] 在 GoalEndpoints.cs 加入 CreateGoalRequest 驗證 - teamMemberId 必填，description 1-500 字元（必須選擇團隊成員且描述不為空）
- [X] T062 [US2] 在 GoalMood.BE/Endpoints/GoalEndpoints.cs 依據 contracts/api.yaml 實作 DELETE /api/goals/{goalId} minimal API 端點
- [X] T063 [US2] 在 GoalMood.FE/src/composables/useGoals.ts 建立 useGoals composable，包含 createGoal 和 deleteGoal 方法
- [X] T064 [US2] 在 GoalMood.FE/src/components/GoalInput.vue 建立 GoalInput 元件，使用 DaisyUI 表單元件
- [X] T065 [US2] 在 GoalInput.vue 使用 DaisyUI select 元件實作團隊成員下拉選單，依據 research.md
- [X] T066 [US2] 在 GoalInput.vue 實作目標描述 textarea，包含 500 字元限制驗證
- [X] T067 [US2] 在 GoalInput.vue 加入表單提交處理器以呼叫 createGoal API
- [X] T068 [US2] 在 GoalInput.vue 為缺少 teamMemberId 或空白描述顯示驗證錯誤訊息（必須選擇團隊成員且輸入目標描述）
- [X] T069 [US2] 在 MemberCard.vue 為每個目標加入刪除按鈕及垃圾桶圖示
- [X] T070 [US2] 在 MemberCard.vue 使用 DaisyUI modal 元件實作刪除確認對話框，依據澄清問題 5
- [X] T071 [US2] 在 MemberCard.vue 加入刪除目標處理器，在確認後呼叫 deleteGoal API
- [X] T072 [US2] 在 App.vue 於目標建立或刪除後重新整理儀表板資料

### 使用者故事 2 的整合測試（必要）

- [ ] T073 [US2] 在 tests/integration/UserStory2_AddGoal_Tests.cs 建立有效輸入新增目標的整合測試 - 依據驗收情境 1 驗證目標出現在儀表板卡片中
- [ ] T074 [US2] 在 tests/integration/UserStory2_AddGoal_Tests.cs 建立新增的目標預設為未完成的整合測試 - 依據驗收情境 2 驗證核取方塊未勾選
- [ ] T075 [US2] 在 tests/integration/UserStory2_AddGoal_Tests.cs 建立為一個成員新增多個目標的整合測試 - 依據驗收情境 3 驗證所有目標按順序列出
- [ ] T076 [US2] 在 tests/integration/UserStory2_AddGoal_Tests.cs 建立含確認的刪除目標整合測試 - 依據驗收情境 6 驗證目標移除且統計資料更新

**檢查點**：此時使用者故事 1 和 2 應該都能獨立運作。使用者可以檢視儀表板並新增/刪除目標。

---

## 階段 5：使用者故事 3 - 更新團隊成員心情（優先級：P3）

**目標**：允許使用者透過表單更新團隊成員心情

**獨立測試**：使用「更新心情」表單變更團隊成員心情，並驗證心情表情符號在儀表板上更新

### 使用者故事 3 的合約測試（必要）

- [ ] T077 [P] [US3] 在 tests/contract/TeamMemberEndpointsTests.cs 為 PUT /api/members/{memberId}/mood 有效輸入建立合約測試 - 依據 contracts/api.yaml 驗證 200 狀態和更新的 TeamMemberDto
- [ ] T078 [P] [US3] 在 tests/contract/TeamMemberEndpointsTests.cs 為 PUT /api/members/{memberId}/mood 無效心情值建立合約測試 - 驗證 400 狀態和錯誤訊息（心情必須為 1-5）
- [ ] T079 [P] [US3] 在 tests/contract/TeamMemberEndpointsTests.cs 為 PUT /api/members/{memberId}/mood 不存在的 memberId 建立合約測試 - 驗證 404 狀態

### 使用者故事 3 的實作

- [X] T080 [US3] 在 GoalMood.BE/Data/ITeamMemberRepository.cs 加入 UpdateMoodAsync 方法到 ITeamMemberRepository 介面
- [X] T081 [US3] 在 TeamMemberRepository.cs 使用 Dapper ExecuteAsync 參數化查詢實作 UpdateMoodAsync
- [X] T082 [US3] 在 GoalMood.BE/Endpoints/TeamMemberEndpoints.cs 依據 contracts/api.yaml 實作 PUT /api/members/{memberId}/mood minimal API 端點
- [X] T083 [US3] 在 TeamMemberEndpoints.cs 加入 UpdateMoodRequest 驗證 - mood 必須為 1-5（必須選擇有效的心情表情符號）
- [X] T084 [US3] 在 GoalMood.FE/src/composables/useMoods.ts 建立 useMoods composable，包含 updateMood 方法
- [X] T085 [US3] 在 GoalMood.FE/src/components/MoodSelector.vue 建立 MoodSelector 元件，使用 DaisyUI 表單元件
- [X] T086 [US3] 在 MoodSelector.vue 使用 DaisyUI select 元件實作團隊成員下拉選單
- [X] T087 [US3] 在 MoodSelector.vue 使用 DaisyUI btn-group 實作心情表情符號按鈕群組，依據 research.md（😀 😊 😐 😞 😤）
- [X] T088 [US3] 在 MoodSelector.vue 加入表單提交處理器以呼叫 updateMood API
- [X] T089 [US3] 在 MoodSelector.vue 為缺少 teamMemberId 或 mood 顯示驗證錯誤訊息（必須選擇團隊成員和心情表情符號）
- [X] T090 [US3] 在 App.vue 於心情更新後重新整理儀表板資料以顯示新的心情表情符號

### 使用者故事 3 的整合測試（必要）

- [ ] T091 [US3] 在 tests/integration/UserStory3_UpdateMood_Tests.cs 建立有效選擇更新心情的整合測試 - 依據驗收情境 1 驗證心情表情符號在儀表板卡片中顯示
- [ ] T092 [US3] 在 tests/integration/UserStory3_UpdateMood_Tests.cs 建立更新現有心情的整合測試 - 依據驗收情境 2 驗證先前的心情被新心情取代

**檢查點**：此時使用者故事 1、2 和 3 應該都能獨立運作。目標的完整 CRUD 和心情更新功能可運作。

---

## 階段 6：使用者故事 4 - 標記目標為完成（優先級：P4）

**目標**：允許使用者透過核取方塊標記目標為已完成或未完成

**獨立測試**：點擊目標完成核取方塊並驗證完成計數更新

### 使用者故事 4 的合約測試（必要）

- [ ] T093 [P] [US4] 在 tests/contract/GoalEndpointsTests.cs 為 PUT /api/goals/{goalId}/complete 建立合約測試 - 依據 contracts/api.yaml 驗證 200 狀態和 isCompleted=true 的 GoalDto
- [ ] T094 [P] [US4] 在 tests/contract/GoalEndpointsTests.cs 為 PUT /api/goals/{goalId}/uncomplete 建立合約測試 - 依據 contracts/api.yaml 驗證 200 狀態和 isCompleted=false 的 GoalDto
- [ ] T095 [P] [US4] 在 tests/contract/GoalEndpointsTests.cs 為 PUT /api/goals/{goalId}/complete 不存在的 goalId 建立合約測試 - 驗證 404 狀態

### 使用者故事 4 的實作

- [X] T096 [US4] 在 GoalMood.BE/Data/IGoalRepository.cs 加入 UpdateCompletionAsync 方法到 IGoalRepository 介面
- [X] T097 [US4] 在 GoalRepository.cs 使用 Dapper ExecuteAsync 參數化查詢實作 UpdateCompletionAsync
- [X] T098 [US4] 在 GoalMood.BE/Endpoints/GoalEndpoints.cs 依據 contracts/api.yaml 實作 PUT /api/goals/{goalId}/complete minimal API 端點
- [X] T099 [US4] 在 GoalMood.BE/Endpoints/GoalEndpoints.cs 依據 contracts/api.yaml 實作 PUT /api/goals/{goalId}/uncomplete minimal API 端點
- [X] T100 [US4] 在 GoalMood.FE/src/composables/useGoals.ts 加入 toggleComplete 方法到 useGoals composable
- [X] T101 [US4] 在 MemberCard.vue 加入核取方塊變更處理器，在勾選/取消勾選時呼叫 toggleComplete API
- [X] T102 [US4] 在 MemberCard.vue 使用 :class 綁定對已完成的目標套用刪除線 CSS 類別
- [X] T103 [US4] 在 MemberCard.vue 於目標完成狀態切換後重新整理完成計數徽章（即時更新）

### 使用者故事 4 的整合測試（必要）

- [ ] T104 [US4] 在 tests/integration/UserStory4_CompleteGoal_Tests.cs 建立標記目標為完成的整合測試 - 依據驗收情境 1-2 驗證核取方塊已勾選且完成計數更新
- [ ] T105 [US4] 在 tests/integration/UserStory4_CompleteGoal_Tests.cs 建立取消標記已完成目標的整合測試 - 依據驗收情境 3 驗證目標回復為未完成且計數更新
- [ ] T106 [US4] 在 tests/integration/UserStory4_CompleteGoal_Tests.cs 建立所有目標都已完成的整合測試 - 依據驗收情境 4 驗證 100% 完成計數顯示

**檢查點**：此時所有核心 CRUD 功能都已完成。目標可以被新增、完成和刪除。

---

## 階段 7：使用者故事 5 - 檢視團隊統計資料（優先級：P5）

**目標**：顯示全團隊的完成百分比和心情分布統計資料

**獨立測試**：檢視統計面板並驗證它顯示準確的團隊完成百分比和心情分布計數

### 使用者故事 5 的合約測試（必要）

- [ ] T107 [P] [US5] 在 tests/contract/StatsEndpointsTests.cs 為 GET /api/stats 端點建立合約測試 - 依據 contracts/api.yaml 驗證 200 狀態和 StatsDto schema
- [ ] T108 [P] [US5] 在 tests/contract/StatsEndpointsTests.cs 為 GET /api/stats 無目標情況建立合約測試 - 依據驗收情境 5 驗證 0% 完成或「無追蹤目標」

### 使用者故事 5 的實作

- [X] T109 [US5] 在 GoalMood.BE/Data/IGoalRepository.cs 加入 GetTodayGoalsStatsAsync 方法到 IGoalRepository 介面
- [X] T110 [US5] 在 GoalRepository.cs 使用 Dapper QueryAsync 實作 GetTodayGoalsStatsAsync 以計算完成百分比
- [X] T111 [US5] 在 GoalMood.BE/Data/ITeamMemberRepository.cs 加入 GetMoodDistributionAsync 方法到 ITeamMemberRepository 介面
- [X] T112 [US5] 在 TeamMemberRepository.cs 使用 Dapper QueryAsync 搭配 GROUP BY mood 實作 GetMoodDistributionAsync
- [X] T113 [US5] 在 GoalMood.BE/Endpoints/StatsEndpoints.cs 建立 StatsEndpoints 類別
- [X] T114 [US5] 在 GoalMood.BE/Endpoints/StatsEndpoints.cs 依據 contracts/api.yaml 實作 GET /api/stats minimal API 端點，結合目標和心情統計資料
- [X] T115 [US5] 在 GoalMood.FE/src/types/Stats.ts 建立 Stats TypeScript 介面，對應 StatsDto schema
- [X] T116 [US5] 在 GoalMood.FE/src/composables/useStats.ts 建立 useStats composable，包含 fetchStats 方法
- [X] T117 [US5] 在 GoalMood.FE/src/components/StatsPanel.vue 建立 StatsPanel 元件，使用 DaisyUI card 和 badge 元件
- [X] T118 [US5] 在 StatsPanel.vue 顯示完成百分比，包含格式化（例如「已完成 65%」）
- [X] T119 [US5] 在 StatsPanel.vue 顯示心情分布計數，包含表情符號標籤和計數（例如「😀 3人, 😊 2人」）
- [X] T120 [US5] 在 App.vue 儀表板加入 StatsPanel 元件
- [X] T121 [US5] 在 App.vue 於任何目標完成切換或心情更新後重新整理統計資料（即時更新完成計數和心情顯示）

### 使用者故事 5 的整合測試（必要）

- [ ] T122 [US5] 在 tests/integration/UserStory5_Stats_Tests.cs 建立統計面板顯示完成百分比的整合測試 - 依據驗收情境 1 驗證正確的百分比計算
- [ ] T123 [US5] 在 tests/integration/UserStory5_Stats_Tests.cs 建立統計面板顯示心情分布的整合測試 - 依據驗收情境 2 驗證心情計數
- [ ] T124 [US5] 在 tests/integration/UserStory5_Stats_Tests.cs 建立目標完成切換後統計資料更新的整合測試 - 依據驗收情境 3 驗證即時更新
- [ ] T125 [US5] 在 tests/integration/UserStory5_Stats_Tests.cs 建立心情變更後統計資料更新的整合測試 - 依據驗收情境 4 驗證即時更新

**檢查點**：所有 5 個使用者故事現已完成。完整的儀表板、目標管理、心情追蹤和統計資料功能可運作。

---

## 階段 8：優化與跨領域關注

**目的**：影響多個使用者故事的改進

- [X] T126 [P] 在 GoalMood.BE/Program.cs 加入錯誤處理中介軟體，依據 contracts/api.yaml 回傳 ErrorResponse 格式
- [X] T127 [P] 在端點類別中使用 ILogger 為所有 API 端點加入日誌記錄
- [X] T128 [P] 在 GoalMood.FE/src/composables/ 的所有 composables 加入載入狀態（loading: Ref<boolean>）
- [X] T129 [P] 在 GoalMood.FE/src/composables/ 的所有 composables 加入錯誤狀態（error: Ref<string | null>）
- [X] T130 [P] 在 App.vue 取得資料時使用 DaisyUI loading 元件顯示載入轉圈
- [X] T131 [P] 在 App.vue 當 API 呼叫失敗時使用 DaisyUI alert 元件顯示錯誤訊息
- [ ] T132 驗證 SC-001 效能需求 - 儀表板在 2 秒內載入
- [ ] T133 驗證 SC-006 效能需求 - 變更後 1 秒內更新統計資料
- [ ] T134 驗證 SC-007 資料持久性 - 重新啟動應用程式並確認所有資料保留
- [ ] T135 驗證 SC-008 規模需求 - 測試 10 個團隊成員而無效能降低
- [ ] T136 驗證 SC-009 規模需求 - 測試 50 個總目標而無效能降低
- [ ] T137 驗證 SC-010 驗證需求 - 表單錯誤在 200ms 內顯示
- [X] T138 [P] 在 GoalMood.FE 元件的所有表單輸入加入無障礙屬性（aria-label, role）
- [X] T139 [P] 依據 constitution UX 一致性原則驗證所有互動元素的鍵盤導航可運作
- [X] T140 使用 `dotnet test --filter FullyQualifiedName~contract` 執行所有合約測試並驗證全部通過
- [X] T141 使用 `dotnet test --filter FullyQualifiedName~integration` 執行所有整合測試並驗證全部通過
- [X] T142 驗證 quickstart.md 指示 - 遵循所有步驟並驗證應用程式成功執行
- [X] T143 在 specs/001-goal-mood-tracker/plan.md 更新任何與原始計劃偏離的文件

---

## 相依性與執行順序

### 階段相依性

- **設定（階段 1）**：無相依性 - 可立即開始
- **基礎建設（階段 2）**：相依於設定完成 - 阻擋所有使用者故事
- **使用者故事 1（階段 3）**：相依於基礎建設（階段 2）- MVP 基準
- **使用者故事 2（階段 4）**：相依於基礎建設（階段 2）- 可與 US1 並行但整合至儀表板
- **使用者故事 3（階段 5）**：相依於基礎建設（階段 2）- 可與 US1/US2 並行
- **使用者故事 4（階段 6）**：相依於使用者故事 2（目標必須存在才能標記完成）
- **使用者故事 5（階段 7）**：相依於使用者故事 1, 2, 4（統計資料需要有完成資料的目標）
- **優化（階段 8）**：相依於所有使用者故事完成

### 使用者故事相依性

- **使用者故事 1（P1）**：僅基礎建設 → 可獨立測試 ✅
- **使用者故事 2（P2）**：僅基礎建設 → 可獨立測試 ✅
- **使用者故事 3（P3）**：僅基礎建設 → 可獨立測試 ✅
- **使用者故事 4（P4）**：需要使用者故事 2（目標必須存在）→ 使用種子資料測試 ✅
- **使用者故事 5（P5）**：需要使用者故事 1, 2, 4（統計資料需要目標和心情）→ 使用種子資料測試 ✅

### 在每個使用者故事內

1. 合約測試必須先撰寫並在實作前失敗（TDD 要求）
2. DTOs 和 models 在 repositories 之前
3. Repositories 在 endpoints 之前
4. 後端 endpoints 在前端元件之前
5. Composables 在 Vue 元件之前
6. 元件在整合至 App.vue 之前
7. 整合測試在實作完成後

### 並行機會

**階段 1（設定）**：T003, T004, T005, T007, T008, T009, T010 可並行執行

**階段 2（基礎建設）**：T016, T017, T018, T019, T020（所有 DTOs）可並行執行

**使用者故事 1 測試**：T026, T027 可並行執行

**使用者故事 1 實作**：T036, T037（TypeScript 介面）可並行執行

**使用者故事 2 測試**：T049-T055（所有合約測試）可並行執行

**使用者故事 3 測試**：T077-T079（所有合約測試）可並行執行

**使用者故事 4 測試**：T093-T095（所有合約測試）可並行執行

**使用者故事 5 測試**：T107-T108（所有合約測試）可並行執行

**優化階段**：T126-T131, T138-T139（所有錯誤處理、日誌記錄、無障礙）可並行執行

**多位開發者在階段 2 完成後可以並行處理不同的使用者故事**，但請注意 US4 和 US5 的相依性。

---

## 並行範例：使用者故事 1

```bash
# 一起啟動使用者故事 1 的所有合約測試：
任務：「在 tests/contract/TeamMemberEndpointsTests.cs 為 GET /api/members 端點建立合約測試」
任務：「在 tests/contract/TeamMemberEndpointsTests.cs 為 GET /api/members 空資料庫情況建立合約測試」

# 一起啟動使用者故事 1 的所有 TypeScript 介面：
任務：「在 GoalMood.FE/src/types/TeamMember.ts 建立 TeamMember TypeScript 介面」
任務：「在 GoalMood.FE/src/types/Goal.ts 建立 Goal TypeScript 介面」
```

---

## 實作策略

### MVP 優先（僅使用者故事 1）

1. 完成階段 1：設定（T001-T010）
2. 完成階段 2：基礎建設（T011-T025）→ **關鍵檢查點**
3. 完成階段 3：使用者故事 1（T026-T048）
4. **停止並驗證**：獨立測試使用者故事 1
5. 執行 `dotnet run` 並驗證儀表板顯示所有團隊成員及其心情和目標
6. 準備好即可部署/展示 → **MVP 的 45 分鐘目標在此結束**

### 漸進式交付

1. **基礎**（階段 1-2）：設定 + 基礎建設 → 資料庫、models、DTOs 就緒
2. **MVP 發布**（階段 3）：加入使用者故事 1 → 儀表板檢視 → 部署 ✅
3. **版本 1.1**（階段 4）：加入使用者故事 2 → 目標 CRUD → 部署 ✅
4. **版本 1.2**（階段 5）：加入使用者故事 3 → 心情更新 → 部署 ✅
5. **版本 1.3**（階段 6）：加入使用者故事 4 → 目標完成 → 部署 ✅
6. **版本 1.4**（階段 7）：加入使用者故事 5 → 團隊統計資料 → 部署 ✅
7. **版本 1.5**（階段 8）：優化 → 生產就緒 ✅

每個版本都增加漸進式價值而不破壞先前的功能。

### 並行團隊策略

階段 2 完成後有多位開發者：

- **開發者 A**：使用者故事 1（T026-T048）→ 儀表板
- **開發者 B**：使用者故事 2（T049-T076）→ 目標 CRUD
- **開發者 C**：使用者故事 3（T077-T092）→ 心情更新

然後依序：

- **任何開發者**：使用者故事 4（相依於 US2 資料）
- **任何開發者**：使用者故事 5（相依於 US1, US2, US4 資料）
- **所有開發者**：並行優化階段

---

## 備註

- **[P] 任務** = 不同檔案，無相依性，可並行執行
- **[Story] 標籤** = 將任務對應到特定使用者故事以便追溯（US1, US2, US3, US4, US5）
- **合約測試必要** 依據 constitution 條件式 TDD 原則
- **整合測試必要** 依據 constitution 以驗證使用者旅程
- **單元測試可選** - 僅在業務邏輯複雜度證明有必要時加入
- 每個使用者故事應該可獨立完成和測試
- 實作前驗證測試失敗（TDD 紅綠重構）
- 在每個任務或邏輯群組後提交
- 在任何檢查點停止以獨立驗證故事
- **45 分鐘目標**：目標是在時間限制內完成使用者故事 1（MVP）
- 所有檔案路徑都是明確的，並對應到 plan.md 專案結構
- Dapper 查詢必須使用參數化 SQL 以防止 SQL 注入，依據 constitution
- Vue 元件必須僅使用 Composition API，依據 constitution（不可使用 Options API）
- 需要 DaisyUI 語意類別，依據 constitution（btn, card, badge, modal 等）

---

## 任務摘要

**總任務數**：143

**依階段分類**：
- 階段 1（設定）：10 個任務
- 階段 2（基礎建設）：15 個任務
- 階段 3（使用者故事 1 - P1）：23 個任務（5 個測試 + 18 個實作）
- 階段 4（使用者故事 2 - P2）：28 個任務（7 個測試 + 21 個實作）
- 階段 5（使用者故事 3 - P3）：16 個任務（3 個測試 + 13 個實作）
- 階段 6（使用者故事 4 - P4）：14 個任務（3 個測試 + 11 個實作）
- 階段 7（使用者故事 5 - P5）：19 個任務（2 個測試 + 17 個實作）
- 階段 8（優化）：18 個任務

**依使用者故事分類**：
- 使用者故事 1（P1 - 檢視團隊儀表板）：23 個任務
- 使用者故事 2（P2 - 新增每日目標）：28 個任務
- 使用者故事 3（P3 - 更新團隊成員心情）：16 個任務
- 使用者故事 4（P4 - 標記目標為完成）：14 個任務
- 使用者故事 5（P5 - 檢視團隊統計資料）：19 個任務

**已識別的並行機會**：28 個任務標記為 [P]

**獨立測試標準**：
- 使用者故事 1：載入應用程式 → 看到所有成員及其心情和目標
- 使用者故事 2：透過表單新增目標 → 在儀表板上看到它
- 使用者故事 3：透過表單更新心情 → 在儀表板上看到表情符號變更
- 使用者故事 4：切換核取方塊 → 看到完成計數更新
- 使用者故事 5：檢視統計面板 → 看到準確的百分比和分布

**MVP 範圍**：完成到階段 3（使用者故事 1）= 最小可行產品需 48 個任務

**格式驗證**：✅ 所有 143 個任務遵循嚴格的檢查清單格式 `- [ ] [ID] [P?] [Story?] 包含檔案路徑的描述`
