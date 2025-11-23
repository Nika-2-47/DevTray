function Get-StaticProperty {
    <#
    .SYNOPSIS
        指定された型の静的プロパティを表示します。
    .DESCRIPTION
        .NETの型名を指定して、その型の静的プロパティ（Static Property）の一覧を取得します。
        "System." プレフィックスは省略可能です。
    .EXAMPLE
        Get-StaticProperty DateTime
    .EXAMPLE
        Get-StaticProperty System.Math
    #>
    param (
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [string]$TypeName
    )
    process {
        try {
            $targetType = $null

            # 入力が既に型オブジェクトの場合はそのまま使う
            if ($TypeName -is [Type]) {
                $targetType = $TypeName
            } else {
                # 型名として解決を試みる
                $targetType = $TypeName -as [Type]
                
                # 解決できない場合、System名前空間を付与して再試行
                if ($null -eq $targetType -and -not $TypeName.Contains(".")) {
                    $targetType = ("System." + $TypeName) -as [Type]
                }
            }

            if ($null -eq $targetType) {
                Write-Error "型 '$TypeName' が見つかりませんでした。"
                return
            }
            
            $targetType | Get-Member -Static -MemberType Property
        }
        catch {
            Write-Error "エラーが発生しました: $_"
        }
    }
}

# テスト実行
Write-Host "--- DateTime のテスト ---"
Get-StaticProperty DateTime

Write-Host "`n--- Math のテスト ---"
Get-StaticProperty Math