import json
from http.server import BaseHTTPRequestHandler, HTTPServer


class AlertReceiver(BaseHTTPRequestHandler):
    def do_POST(self):
        length = int(self.headers.get("Content-Length", "0"))
        payload = json.loads(self.rfile.read(length) or b"{}")
        print(json.dumps(payload, ensure_ascii=False), flush=True)
        self.send_response(202)
        self.end_headers()

    def do_GET(self):
        self.send_response(200)
        self.end_headers()
        self.wfile.write(b"alert receiver is ready")

    def log_message(self, format, *args):
        return


HTTPServer(("0.0.0.0", 8080), AlertReceiver).serve_forever()
