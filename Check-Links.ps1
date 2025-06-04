Param(
    [Parameter(Mandatory = $true)]
    [string]$MarkdownFile,

    [Parameter(Mandatory = $false)]
    [string]$IgnoreListFile
)

# ファイルの存在確認
if (-not (Test-Path $MarkdownFile)) {
    Write-Error "エラー: Markdownファイルが見つかりません - '$MarkdownFile'"
    exit 1
}

# 無視するURL一覧の読み込み（もし指定されていれば）
$ignoreUrls = @()
if ($IgnoreListFile) {
    if (-not (Test-Path $IgnoreListFile)) {
        Write-Error "エラー: 無視リストファイルが見つかりません - '$IgnoreListFile'"
        exit 1
    }
    $ignoreUrls = Get-Content -Path $IgnoreListFile | Where-Object { $_ -match '^https?://' } | ForEach-Object { $_.Trim() }
}

# ファイル内容の読み込み
$content = Get-Content -Path $MarkdownFile -Raw

# ハイパーリンクの正規表現パターン
$pattern = '\[(.*?)\]\((http[s]?://.*?)\)'

# ハイパーリンクの抽出
$matches = [regex]::Matches($content, $pattern)

if ($matches.Count -eq 0) {
    Write-Host "情報: ハイパーリンクが見つかりませんでした。"
    exit 0
}

$errors = @()
$totalLinks = $matches.Count
$currentLink = 1

# User-Agent ヘッダーを設定
$headers = @{
    'User-Agent' = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36'
}

foreach ($match in $matches) {
    $linkText = $match.Groups[1].Value
    $url = $match.Groups[2].Value

    if ($ignoreUrls -contains $url) {
        Write-Host "[$currentLink/$totalLinks] スキップ: $url (無視リストに登録されているため)"
        $currentLink++
        continue
    }

    if ($linkText -like "*`(リンク切れ`)*") {
        Write-Host "[$currentLink/$totalLinks] スキップ: $url (リンクテキストに「(リンク切れ)」を含むため)"
        $currentLink++
        continue
    }

    Write-Host "[$currentLink/$totalLinks] チェック中: $url"

    $success = $false
    $methods = @('HEAD', 'GET')

    foreach ($method in $methods) {
        $retryCount = 0
        while ($retryCount -lt 5) {
            try {
                $response = Invoke-WebRequest -Uri $url -Method $method -Headers $headers -UseBasicParsing -TimeoutSec 15 -ErrorAction Stop
                if ($response.StatusCode -lt 400) {
                    Write-Host "  結果: OK ($($response.StatusCode)) - メソッド: $method" -ForegroundColor Green
                    $success = $true
                    break
                } else {
                    Write-Host "  結果: エラー ($($response.StatusCode)) - メソッド: $method" -ForegroundColor Yellow
                }
                break  # 400以上ならリトライしない
            } catch {
                if ($_.Exception.Response?.StatusCode.Value__ -eq 429) {
                    Write-Host "  結果: 429 Too Many Requests - リトライ中 ($($retryCount + 1)/5) - メソッド: $method" -ForegroundColor Yellow
                    Start-Sleep -Seconds 5
                    $retryCount++
                } else {
                    Write-Host "  結果: エラー ($($_.Exception.Message)) - メソッド: $method" -ForegroundColor Red
                    break
                }
            }
        }

        if ($success) { break }
    }

    if (-not $success) {
        $errors += @{ URL = $url; Message = "アクセスに失敗しました。" }
    }

    Start-Sleep -Milliseconds 1000  # 緩衝用の軽い遅延
    $currentLink++
}

if ($errors.Count -gt 0) {
    Write-Host "`n以下のリンクでエラーが発生しました:" -ForegroundColor Yellow
    foreach ($linkError in $errors) {
        Write-Host "- $($linkError.URL): $($linkError.Message)" -ForegroundColor Red
    }
    exit 1
} else {
    Write-Host "`nすべてのリンクが有効です。" -ForegroundColor Green
}
