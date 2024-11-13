from http.server import HTTPServer, SimpleHTTPRequestHandler

class CORSRequestHandler(SimpleHTTPRequestHandler):
    def end_headers(self):
        # 添加CORS头
        self.send_header('Access-Control-Allow-Origin', '*')
        self.send_header('Access-Control-Allow-Methods', 'GET')
        self.send_header('Access-Control-Allow-Headers', 'Content-Type')
        super().end_headers()

if __name__ == '__main__':
    server_address = ('', 9999)  # 监听所有IP地址，端口9999
    httpd = HTTPServer(server_address, CORSRequestHandler)
    print("Serving on port 9999 with CORS enabled")
    httpd.serve_forever()
