import {DownloadAndAssembleVideoUseCase} from '../application/useCases/DownloadAndAssembleVideoUseCase.ts';
import {PuppeteerDownloader} from '../infrastructure/puppeteer/puppeteerDownloader.ts';
import {FfmpegAssembler} from '../infrastructure/ffmpeg/ffmpegAssembler.ts';

const useCase = new DownloadAndAssembleVideoUseCase(
  new PuppeteerDownloader(),
  new FfmpegAssembler()
);

const prompt = "Enter the X link: ";
process.stdout.write(prompt);
for await (const line of console) {
  await useCase.execute(line);
}
