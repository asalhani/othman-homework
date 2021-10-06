//the backend unit test is not implemented yet DO NOT FORGET TO ADD IT
import { UserToCreate } from './_interfaces/userToCreate.model';
import { User } from './_interfaces/user.model';
import { FileInfo } from './_interfaces/file.info.model';
import { FileCategory } from './_interfaces/fileCategory.model';
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
  public filesList : FileCategory[] = [];

  public photos: string[] = [];

  constructor(private http: HttpClient, private fileService: FileService) 
  {
    this.filesList = [];
   }

  ngOnInit() {
    this.isCreate = true;
    this.fileService.getFilesList().subscribe(data =>{
      let files = data as Array<FileInfo>; //casting
      if (files.length > 0)
      {
        const fileTypes = [...new Set(files.map(item => item.contentType))];
        if(fileTypes && fileTypes.length > 0)
        {
          for (let i = 0; i < fileTypes.length; i++)
          {
            let categoryFiles = [];
            for (let j = 0; j < files.length; j++)
            {
              if (fileTypes[i] == files[j].contentType)
              {
                categoryFiles.push(files[j]);
              }
            }
            if (categoryFiles.length > 0)
            {
              let category = {} as FileCategory;
              category.contentType = fileTypes[i];
              category.files = categoryFiles;
              this.filesList.push(category);
            }
          }
        }
      }
      console.log(this.filesList)
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
