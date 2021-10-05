import { UserToCreate } from './_interfaces/userToCreate.model';
import { User } from './_interfaces/user.model';
import { FileInfo } from './_interfaces/file.info.model';
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FileService } from './_services/file.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  public isCreate: boolean;
  public name: string;
  public address: string;
  public user: UserToCreate;
  public users: User[] = [];
  public response: {dbPath: ''};
  public filesList : FileInfo[] = [];

  public photos: string[] = [];

  constructor(private http: HttpClient, private fileService: FileService) 
  {
    this.filesList = [];
   }

  ngOnInit() {
    this.isCreate = true;
    // this.getPhotos();
    this.fileService.getFilesList().subscribe(data =>{
      this.filesList = data as Array<FileInfo>;
      console.log(data)
    });

  }

  private getPhotos = () => {
    this.fileService.getPhotos().subscribe(data => this.photos = data['photos']);
  }

  public onCreate = () => {
    this.user = {
      name: this.name,
      address: this.address,
      imgPath: this.response.dbPath
    }

    this.http.post('https://localhost:5001/api/users', this.user)
      .subscribe(res => {
        this.getUsers();
        this.isCreate = false;
    });
  }

  private getUsers = () => {
    this.http.get('https://localhost:5001/api/users')
    .subscribe(res => {
      this.users = res as User[];
    });
  }

  public returnToCreate = () => {
    this.isCreate = true;
    this.name = '';
    this.address = '';
    this.getPhotos();
  }

  public uploadFinished = (event) => {
    this.response = event;
  }

  public createImgPath = (serverPath: string) => {
    return `http://localhost:5000/${serverPath}`;
  }
}
