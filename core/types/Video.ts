import type {VideoMetadata} from './VideoMetadata.ts';

export interface Video {
  Buffer: Uint8Array[],
  metadata: VideoMetadata
}