#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>

// OLED設定 (128x32ピクセル, I2Cアドレス 0x3C)
#define SCREEN_WIDTH 128
#define SCREEN_HEIGHT 32
#define OLED_RESET -1
Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, OLED_RESET);

const int LED_PIN = 13;
const int BTN_LANG1 = A0;
const int BTN_LANG2 = A1;

// 状態管理用変数
char lastCommand[10] = "None";
char currentStatus[20] = "Ready";

void setup() {
  // 【重要】最初にシリアル通信を開始する
  Serial.begin(9600);
  
  // 起動直後の安定化待ち
  delay(1000);
  Serial.println("=== Setup Start ===");

  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW);

  pinMode(BTN_LANG1, INPUT);
  pinMode(BTN_LANG2, INPUT);

  // OLED初期化
  // 失敗しても無限ループで止めず、シリアルにエラーを出して続行させる
  if(!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) {
    Serial.println(F("SSD1306 allocation failed - Check wiring!"));
    // ここで for(;;); をするとUSB通信も死ぬ可能性があるので削除
  } else {
    Serial.println("OLED Initialized");
    display.clearDisplay();
    display.setTextSize(1);
    display.setTextColor(SSD1306_WHITE);
    updateDisplay();
  }

  // while (!Serial) {} // ← これがあるとPC接続まで起動しないので、削除推奨
  
  Serial.println("=== Nano: Button Lang1 / Lang2 Sender ===");
}

void loop() {
  if (digitalRead(BTN_LANG1) == HIGH) {
    sendCommand("Lang1");
    delay(300);
  }

  if (digitalRead(BTN_LANG2) == HIGH) {
    sendCommand("Lang2");
    delay(300);
  }
}

// OLED表示関数
void displayStatus(const char* line1, const char* line2 = "") {
  // OLEDが初期化失敗していても、ライブラリ側で無視されるはずだが念のためチェックしてもよい
  display.clearDisplay();
  display.setCursor(0, 0);
  display.println(line1);
  if (strlen(line2) > 0) {
    display.setCursor(0, 16);
    display.println(line2);
  }
  display.display();
}

// 常時状態表示関数
void updateDisplay() {
  display.clearDisplay();
  display.setTextSize(1);
  display.setCursor(0, 0);
  display.println("Lang Switcher");
  
  display.setCursor(0, 10);
  display.print("Status: ");
  display.println(currentStatus);
  
  display.setCursor(0, 20);
  display.print("Last: ");
  display.println(lastCommand);
  
  display.display();
}

void sendCommand(const char* cmd) {
  Serial.print("--- Sending: ");
  Serial.print(cmd);
  Serial.println(" ---");
  Serial.println(cmd); // 実際のコマンド送信
  Serial.println("--- Sent ---");

  strcpy(currentStatus, "Sending");
  updateDisplay();

  digitalWrite(LED_PIN, HIGH);
  delay(150);
  digitalWrite(LED_PIN, LOW);

  strcpy(currentStatus, "Sent");
  strcpy(lastCommand, cmd);
  updateDisplay();
  
  delay(500); 
  
  strcpy(currentStatus, "Ready");
  updateDisplay();
}