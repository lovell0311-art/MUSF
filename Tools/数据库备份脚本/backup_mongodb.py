import os
import shutil
import time
import zipfile



def compressFolder(folderPath, compressPathName):
    '''
    :param folderPath: 文件夹路径
    :param compressPathName: 压缩包路径
    :return:
    '''
    zip = zipfile.ZipFile(compressPathName, 'w', zipfile.ZIP_DEFLATED)

    for path, dirNames, fileNames in os.walk(folderPath):
        fpath = path.replace(folderPath, '')
        for name in fileNames:
            fullName = os.path.join(path, name)

            name = fpath + '\\' + name
            zip.write(fullName, name)

    zip.close()





mongodb_path = "D:/MongoDB/Server/4.0/bin/"
backup_path = "D:/mongodb_backup/"
mongodb_host = "192.168.1.176:58030"

ymd = time.strftime("%Y%m%d%H%M%S", time.localtime())

print("MongoDB数据库备份")
print("*****************************")
print("备份目录 " + ymd)
print("*****************************")


mu_ET_code = mongodb_path + "mongodump --host " + mongodb_host + " --db ET --out " + backup_path + ymd
mu_ET1_code = mongodb_path + "mongodump --host " + mongodb_host + " --db ET1 --out " + backup_path + ymd
mu_ET2_code = mongodb_path + "mongodump --host " + mongodb_host + " --db ET2 --out " + backup_path + ymd

if not os.path.exists(backup_path) :
    os.mkdir(backup_path)

os.mkdir(backup_path + ymd)

os.system(mu_ET_code)
os.system(mu_ET1_code)
os.system(mu_ET2_code)

print("压缩备份文件")
compressFolder(backup_path + ymd,backup_path + ymd + ".zip")

shutil.rmtree(backup_path + ymd)
print("MongoDB数据库备份 完成")