export class FileStreamHelper {
  constructor() {
  }

  static async readAllStream(stream: ReadableStream<Uint8Array>): Promise<Uint8Array[]> {
    const reader = stream.getReader();
    const chunks: Uint8Array[] = []

    while (true) {
      const {value, done} = await reader.read();
      if (done) break;

      if (value) chunks.push(value);
    }

    return chunks;
  }
}