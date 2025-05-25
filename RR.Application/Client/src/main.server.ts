import { platformServer } from '@angular/platform-server';
import { AppModule } from './app/app.module';

export default () => {
  return platformServer().bootstrapModule(AppModule);
}
