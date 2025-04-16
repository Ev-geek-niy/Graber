export interface IAssembler {
  getVideoBuffer(url: string): Promise<Uint8Array[]>;
}