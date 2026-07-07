import sys
import os

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PROMPT_FILE = os.path.join(SCRIPT_DIR, "alerts_prompt.txt")

with open(PROMPT_FILE, "r", encoding="utf-8") as f:
    SYSTEM_INSTRUCTION = f.read()

def analyze_alerts(llm, review_text):
    final_prompt = SYSTEM_INSTRUCTION.replace("{opinia}", review_text)
    
    try:
        response = llm(
            final_prompt,
            max_tokens=512,
            temperature=0.1
        )
        raw_output = response['choices'][0]['text']
        clean_alerts_output = raw_output.replace("\n", " ").replace("\r", " ").strip()
        return clean_alerts_output
    except Exception as e:
        return f'{{"error": "{str(e)}"}}'