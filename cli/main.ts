import {DownloadAndAssembleVideoUseCase} from '../application/useCases/DownloadAndAssembleVideoUseCase.ts';
import {PuppeteerDownloader} from '../infrastructure/puppeteer/puppeteerDownloader.ts';
import {FfmpegAssembler} from '../infrastructure/ffmpeg/ffmpegAssembler.ts';

const downloadAndAssembleUseCase = new DownloadAndAssembleVideoUseCase(
  new PuppeteerDownloader(),
  new FfmpegAssembler()
);

const prompt = 'Enter the X link (q for quit): ';
process.stdout.write(prompt);
for await (const line of console) {
  if (line === 'q')
    break;
  try {
    await downloadAndAssembleUseCase.execute(line);
    process.stdout.write(prompt);
  }
  catch (err) {
    if (err instanceof Error) {
      console.error(err);
    }
    process.stdout.write(prompt);
  }
}
