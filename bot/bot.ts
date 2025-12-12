import {Bot, InputFile} from 'grammy';
import Xregex from '../shared/regex/Xregex.ts';
import {loadEnv} from './env.ts';
import {PuppeteerWebTracerService} from '../infrastructure/services/puppeteerWebTracerService.ts';
import {FfmpegVideoService} from '../infrastructure/services/ffmpegVideoService.ts';
import {log} from '../shared/logger.ts';
import {CreateTempVideoFileFromHlsUseCase} from '../application/createTempVideoFileFromHls.useCase.ts';
import {Proxy} from '../core/models/Proxy.ts';
import {NullProxy} from '../core/models/NullProxy.ts';

loadEnv()
const proxy = process.env.PROXY_ADDRESS && process.env.PROXY_PORT
  ? new Proxy({
      address: process.env.PROXY_ADDRESS!,
      port: process.env.PROXY_PORT!,
      username: process.env.PROXY_USERNAME,
      password: process.env.PROXY_PASSWORD,
    })
  : new NullProxy();


const videoCreatorInFolder = new CreateTempVideoFileFromHlsUseCase(
  new PuppeteerWebTracerService(),
  new FfmpegVideoService(proxy),
  proxy,

)

const bot = new Bot(process.env.BOT_TOKEN!);

bot.command('start', async (ctx) => {
  await ctx.reply('Welcome lets goooo');
})

bot.on('message:entities:url', async (ctx) => {
  const url = ctx.message.text;

  log(`Проверка URL: ${url}`)

  if (!Xregex.XUrlRegex.test(url)) {
    await bot.api.sendMessage(
      ctx.message.chat.id,
      'Invalid X url'
    )
  }

  const {filePath, metadata} = await videoCreatorInFolder.execute(url);

  log(`Отправка видео-файла: ${filePath}`)

  await bot.api.sendVideo(
    ctx.message.chat.id,
    new InputFile(filePath),
    {
      height: metadata.height,
      width: metadata.width,
      duration: Math.round(+metadata.duration),
      supports_streaming: true,
    }
  )

  log(`Удаление файла: ${filePath}`)
  const file = Bun.file(filePath);
  await file.delete();
  log('Готово!')
})

log('The bot is start working')
bot.start()