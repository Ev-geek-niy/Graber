import {Bot, InputFile} from 'grammy';
import Xregex from '../shared/regex/Xregex.ts';
import {getVideoBufferFromUrlUseCase} from '../application/useCases/getVideoBufferFromUrlUseCase.ts';
import {PuppeteerDownloader} from '../infrastructure/puppeteer/puppeteerDownloader.ts';
import {FfmpegAssembler} from '../infrastructure/ffmpeg/ffmpegAssembler.ts';
import {loadEnv} from './env.ts';


const mediaSender = new getVideoBufferFromUrlUseCase(
  new PuppeteerDownloader(),
  new FfmpegAssembler()
)

loadEnv()
const bot = new Bot(process.env.BOT_TOKEN!);

bot.command('start', async (ctx) => {
  await ctx.reply('Welcome lets goooo');
})

bot.on('message:entities:url', async (ctx) => {
  const url = ctx.message.text;

  if (!Xregex.XUrlRegex.test(url)) {
    await bot.api.sendMessage(
      ctx.message.chat.id,
      'Invalid X url'
    )
  }

  const videoBuffer = await mediaSender.execute(url);
  const fileData = new InputFile(videoBuffer, 'video.mp4')
  await bot.api.sendVideo(
    ctx.message.chat.id,
    fileData
  )
})

bot.start()