export interface IAssembler {
  AssembleVideoByHLCUrl(url: string, outputPath?: string, filename?: string): Promise<void>;
}