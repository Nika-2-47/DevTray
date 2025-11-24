import os
import sys
import time
import yaml
from dotenv import load_dotenv
from playwright.sync_api import sync_playwright

# Load environment variables
load_dotenv()

def run_automation(config_path, site_name):
    if not os.path.exists(config_path):
        print(f"Configuration file not found: {config_path}")
        return

    with open(config_path, 'r', encoding='utf-8') as f:
        config = yaml.safe_load(f)

    if site_name not in config['sites']:
        print(f"Site '{site_name}' not found in configuration.")
        print("Available sites:", ", ".join(config['sites'].keys()))
        return

    site_config = config['sites'][site_name]
    steps = site_config.get('steps', [])

    with sync_playwright() as p:
        browser_type = site_config.get('browser', 'chromium')
        headless = site_config.get('headless', False)
        
        print(f"Launching {browser_type} (Headless: {headless})...")
        
        if browser_type == 'firefox':
            browser = p.firefox.launch(headless=headless)
        elif browser_type == 'webkit':
            browser = p.webkit.launch(headless=headless)
        else:
            # Bot検知回避のための引数を追加
            browser = p.chromium.launch(
                headless=headless,
                args=['--disable-blink-features=AutomationControlled']
            )

        # 一般的なブラウザのUser-Agentを設定してコンテキストを作成
        context = browser.new_context(
            user_agent='Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36'
        )
        page = context.new_page()

        # navigator.webdriver プロパティを隠蔽
        page.add_init_script("""
            Object.defineProperty(navigator, 'webdriver', {
                get: () => undefined
            })
        """)

        print(f"Starting automation for: {site_name}")

        for i, step in enumerate(steps):
            action = step.get('action')
            target = step.get('target') # Selector or URL
            value = step.get('value')
            description = step.get('description', f"Step {i+1}: {action}")

            print(f"Running: {description}")

            try:
                if action == 'goto':
                    page.goto(target)
                
                elif action == 'fill':
                    # Resolve environment variables if value starts with $
                    if value and isinstance(value, str) and value.startswith('$'):
                        env_var = value[1:]
                        fill_value = os.getenv(env_var)
                        if not fill_value:
                            print(f"Warning: Environment variable {env_var} not found.")
                            fill_value = ""
                    else:
                        fill_value = str(value)
                    page.fill(target, fill_value)

                elif action == 'click':
                    page.click(target)

                elif action == 'wait':
                    # Wait for specific time (seconds) or selector
                    if isinstance(value, (int, float)):
                        time.sleep(value)
                    elif isinstance(value, str):
                        page.wait_for_selector(value)
                
                elif action == 'screenshot':
                    page.screenshot(path=value)

                elif action == 'press':
                    page.press(target, value)
                
                elif action == 'select_option':
                    page.select_option(target, value)

                elif action == 'check':
                    page.check(target)
                
                elif action == 'uncheck':
                    page.uncheck(target)

                else:
                    print(f"Unknown action: {action}")

                # Optional: Wait after each step
                wait_after = step.get('wait_after', 0.5)
                if wait_after > 0:
                    time.sleep(wait_after)

            except Exception as e:
                print(f"Error executing step '{description}': {e}")
                # Stop execution on error unless 'ignore_error' is true
                if not step.get('ignore_error', False):
                    break

        print("Automation finished.")
        browser.close()

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python automator.py <site_name> [config_file]")
        print("Example: python automator.py attendance config/sites.yaml")
        sys.exit(1)

    site_name = sys.argv[1]
    config_file = sys.argv[2] if len(sys.argv) > 2 else "config/sites.yaml"

    run_automation(config_file, site_name)
