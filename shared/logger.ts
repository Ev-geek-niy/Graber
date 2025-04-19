export function log(message: string): void {
  console.log(`[LOG]: ${message}`);
}

export function logWarning(message: string): void {
  console.warn(`[WARNING]: ${message}`);
}

export function logError(message: string): void {
  console.error(`[ERROR]: ${message}`);
}