Param(
    [Parameter(Mandatory = $true)]
    [string]$MarkdownFile
)

# ファイルの存在確認
if (-not (Test-Path $MarkdownFile)) {
    Write-Error "エラー: ファイルが見つかりません - '$MarkdownFile'"
    exit 1
}

# ファイル内容の読み込み
$content = Get-Content -Path $MarkdownFile -Raw

# ハイパーリンクの正規表現パターン
$pattern = '\[.*?\]\((http[s]?://.*?)\)'

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
    'User-Agent' = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)'
}

foreach ($match in $matches) {
    $url = $match.Groups[1].Value
    Write-Host "[$currentLink/$totalLinks] チェック中: $url"

    $success = $false

    # メソッドのリスト（HEAD を試してから GET）
    $methods = @('HEAD', 'GET')

    foreach ($method in $methods) {
        try {
            $response = Invoke-WebRequest -Uri $url -Method $method -Headers $headers -UseBasicParsing -TimeoutSec 15 -ErrorAction Stop

            # ステータスコードが 400 未満なら成功とみなす
            if ($response.StatusCode -lt 400) {
                Write-Host "  結果: OK ($($response.StatusCode)) - メソッド: $method" -ForegroundColor Green
                $success = $true
                break
            } else {
                Write-Host "  結果: エラー ($($response.StatusCode)) - メソッド: $method" -ForegroundColor Yellow
            }
        } catch {
            Write-Host "  結果: エラー ($($_.Exception.Message)) - メソッド: $method" -ForegroundColor Red
        }
    }

    if (-not $success) {
        $errors += @{
            URL = $url
            Message = "アクセスに失敗しました。"
        }
    }

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
