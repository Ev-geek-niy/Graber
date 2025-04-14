export enum TweetTypeEnum {
    VIDEO = 'video',
    GIF = 'gif',
    TEXT = 'text',
}

export interface Tweet {
    url: string;
    type: TweetTypeEnum;
}