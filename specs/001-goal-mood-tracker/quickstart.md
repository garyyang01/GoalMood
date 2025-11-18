# 快速開始指南：團隊每日目標與心情追蹤器

**功能**: 001-goal-mood-tracker
**日期**: 2025-11-19
**目標受眾**: 開發人員

## 概述

本指南提供快速設定和運行 GoalMood 應用程式的步驟。預計完成時間：10-15 分鐘。

---

## 前置需求

### 必要工具

- **.NET 8 SDK**: [下載連結](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+**: [下載連結](https://nodejs.org/)
- **Git**: 版本控制

### 驗證安裝

```bash
# 驗證 .NET 8
dotnet --version
# 預期輸出: 8.0.x

# 驗證 Node.js
node --version
# 預期輸出: v18.x.x 或更高

# 驗證 npm
npm --version
# 預期輸出: 9.x.x 或更高
```

---

## 後端設定（GoalMood.BE）

### 1. 導航到後端目錄

```bash
cd GoalMood.BE
```

### 2. 還原 NuGet 套件

```bash
dotnet restore
```

### 3. 執行資料庫遷移

```bash
# 執行初始 schema
dotnet run --migrate

# 或手動執行（如果上述命令不可用）
sqlite3 GoalMood.db < Migrations/001_InitialSchema.sql
```

### 4. （可選）填充測試資料

```bash
# 如果 Program.cs 有提供 seed 選項
dotnet run --seed

# 或使用 SQL 直接插入
sqlite3 GoalMood.db < Migrations/seed-data.sql
```

### 5. 運行後端

```bash
# 開發模式（啟用 hot reload）
dotnet watch run

# 或一般模式
dotnet run
```

**預期輸出**:
```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

### 6. 驗證後端

開啟瀏覽器訪問: `http://localhost:5000/swagger`

應該看到 Swagger UI 顯示所有 API endpoints。

---

## 前端設定（GoalMood.FE）

### 1. 建立 Vue 3 專案

```bash
# 在專案根目錄
npm create vite@latest GoalMood.FE -- --template vue-ts
cd GoalMood.FE
```

### 2. 安裝依賴項

```bash
# 安裝核心依賴
npm install

# 安裝 DaisyUI 和 Tailwind CSS
npm install -D tailwindcss postcss autoprefixer daisyui
npx tailwindcss init -p
```

### 3. 配置 Tailwind CSS

編輯 `tailwind.config.js`:

```javascript
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {},
  },
  plugins: [require("daisyui")],
}
```

### 4. 配置 API 基礎 URL

建立 `.env.local`:

```env
VITE_API_BASE_URL=http://localhost:5000/api
```

### 5. 運行前端

```bash
npm run dev
```

**預期輸出**:
```
VITE v5.x.x  ready in 500 ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
```

### 6. 驗證前端

開啟瀏覽器訪問: `http://localhost:5173`

應該看到應用程式載入且無主控台錯誤。

---

## 完整開發環境

### 同時運行前後端

**終端機 1（後端）**:
```bash
cd GoalMood.BE
dotnet watch run
```

**終端機 2（前端）**:
```bash
cd GoalMood.FE
npm run dev
```

### 使用 tmux/screen（可選）

```bash
# 使用 tmux 分割視窗
tmux new-session -s goalmood
# Ctrl+B 然後 % 分割垂直視窗
# 左邊運行後端，右邊運行前端
```

---

## 資料庫管理

### 檢視資料庫

```bash
# 使用 SQLite CLI
sqlite3 GoalMood.db

# 常用命令
.tables                    # 列出所有表
.schema TeamMembers        # 檢視表結構
SELECT * FROM TeamMembers; # 查詢資料
.quit                      # 退出
```

### 重置資料庫

```bash
# 刪除資料庫檔案
rm GoalMood.db

# 重新運行遷移
dotnet run --migrate
dotnet run --seed  # 可選
```

### 使用 GUI 工具（可選）

推薦工具：
- **DB Browser for SQLite**: [下載連結](https://sqlitebrowser.org/)
- **TablePlus**: [下載連結](https://tableplus.com/)

---

## 測試執行

### 後端測試

```bash
# 在專案根目錄
cd tests

# 運行所有測試
dotnet test

# 運行特定類別的測試
dotnet test --filter "FullyQualifiedName~GoalEndpointsTests"

# 顯示詳細輸出
dotnet test --logger "console;verbosity=detailed"
```

### 前端測試（如已實作）

```bash
cd GoalMood.FE

# 運行單元測試
npm run test:unit

# 運行測試並監視變更
npm run test:unit -- --watch
```

---

## 常見問題排解

### 問題 1: 後端無法啟動 - 埠號已被佔用

**錯誤訊息**:
```
Unable to bind to http://localhost:5000 on the IPv4 loopback interface: 'Address already in use'.
```

**解決方案**:
```bash
# 找出佔用埠號的程序
lsof -i :5000  # macOS/Linux
netstat -ano | findstr :5000  # Windows

# 終止該程序或更改埠號
# 編輯 appsettings.json 或使用環境變數
export ASPNETCORE_URLS="http://localhost:5002;https://localhost:5003"
dotnet run
```

### 問題 2: 資料庫檔案找不到

**錯誤訊息**:
```
SQLite Error 14: 'unable to open database file'.
```

**解決方案**:
```bash
# 確認在正確目錄（GoalMood.BE）
pwd

# 檢查 appsettings.json 中的連線字串
cat appsettings.json | grep DefaultConnection

# 手動建立資料庫
sqlite3 GoalMood.db < Migrations/001_InitialSchema.sql
```

### 問題 3: 前端 API 請求失敗（CORS 錯誤）

**錯誤訊息（瀏覽器主控台）**:
```
Access to fetch at 'http://localhost:5000/api/members' from origin 'http://localhost:5173' has been blocked by CORS policy
```

**解決方案**:

確認 `Program.cs` 中已啟用 CORS:

```csharp
// 在 Program.cs 中
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ...

app.UseCors("AllowFrontend");
```

### 問題 4: Tailwind 樣式未套用

**症狀**: 頁面顯示但無樣式

**解決方案**:

確認 `src/main.ts` 或 `src/App.vue` 中已匯入 Tailwind:

```typescript
// src/main.ts
import './style.css'  // 或 './index.css'
```

確認 `style.css` 包含 Tailwind directives:

```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

---

## 開發工作流程

### 1. 建立新功能

```bash
# 確保在 feature branch
git checkout 001-goal-mood-tracker

# 開始開發
# 1. 更新 data model（如需要）
# 2. 更新 API endpoints
# 3. 更新前端組件
# 4. 撰寫測試
# 5. 執行測試
```

### 2. 測試 API endpoints

使用 Swagger UI (`http://localhost:5000/swagger`) 或 cURL:

```bash
# 取得所有團隊成員
curl http://localhost:5000/api/members

# 新增目標
curl -X POST http://localhost:5000/api/goals \
  -H "Content-Type: application/json" \
  -d '{"teamMemberId": 1, "description": "Test goal"}'

# 更新心情
curl -X PUT http://localhost:5000/api/members/1/mood \
  -H "Content-Type: application/json" \
  -d '{"mood": 1}'
```

### 3. 除錯

**後端除錯**:
- 使用 Visual Studio Code 的 C# 除錯器
- 在 `launch.json` 中設定中斷點

**前端除錯**:
- 使用瀏覽器開發者工具
- Vue DevTools 擴充功能

---

## 效能驗證

### 檢查 API 回應時間

```bash
# 使用 curl 測量回應時間
curl -w "@curl-format.txt" -o /dev/null -s http://localhost:5000/api/members

# curl-format.txt 內容:
# time_total: %{time_total}s\n
```

**預期結果**: <100ms（符合規格 SC-001）

### 檢查前端載入時間

開啟瀏覽器開發者工具 → Network 標籤 → 重新載入

**預期結果**: Initial load <2s（符合規格）

---

## 下一步

完成快速開始後，請參閱：

1. **[data-model.md](./data-model.md)**: 深入了解資料結構
2. **[contracts/api.yaml](./contracts/api.yaml)**: 完整 API 規格
3. **[tasks.md](./tasks.md)**: 實作任務清單（使用 `/speckit.tasks` 生成）

---

## 支援資源

- **.NET 8 文件**: https://learn.microsoft.com/en-us/dotnet/
- **Dapper 文件**: https://github.com/DapperLib/Dapper
- **Vue 3 文件**: https://vuejs.org/
- **DaisyUI 文件**: https://daisyui.com/
- **SQLite 文件**: https://www.sqlite.org/docs.html

---

## 聯絡資訊

如有問題或需要協助，請參閱專案 README 或聯絡團隊。
