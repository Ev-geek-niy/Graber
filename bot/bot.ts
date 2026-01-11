import { Bot, InputFile } from "grammy";
import Xregex from "../shared/regex/Xregex.ts";
import { loadEnv } from "./env.ts";
import { log } from "../shared/logger.ts";
import { createVideoCreatorUseCase } from "../infrastructure/container.ts";

loadEnv();

const videoCreatorInFolder = createVideoCreatorUseCase();

const bot = new Bot(process.env.BOT_TOKEN!);

bot.command("start", async (ctx) => {
  await ctx.reply("Welcome lets goooo");
});

bot.on("message:entities:url", async (ctx) => {
  const url = ctx.message.text;

  log(`Проверка URL: ${url}`);

  if (!Xregex.XUrlRegex.test(url)) {
    await bot.api.sendMessage(ctx.message.chat.id, "Invalid X url");
  }

  const message = await bot.api.sendMessage(
    ctx.message.chat.id,
    "Скачивание...",
  );

  const { filePath, metadata } = await videoCreatorInFolder.execute(url);

  log(`Отправка видео-файла: ${filePath}`);

  await bot.api.deleteMessage(ctx.message.chat.id, message.message_id);

  await bot.api.sendVideo(ctx.message.chat.id, new InputFile(filePath), {
    height: metadata.height,
    width: metadata.width,
    duration: Math.round(+metadata.duration),
    supports_streaming: true,
  });

  log(`Удаление файла: ${filePath}`);
  const file = Bun.file(filePath);
  await file.delete();
  log("Готово!");
});

log("The bot is start working");
bot.start();
