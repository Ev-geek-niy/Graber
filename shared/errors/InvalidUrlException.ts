export class InvalidUrlException extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'InvalidUrlException';
    Object.setPrototypeOf(this, InvalidUrlException.prototype);
  }
}