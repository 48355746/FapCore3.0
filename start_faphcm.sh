#cd fapcore
cd /var/lib/jenkins-home/workspace/fapcore30
#stop web container [fapcorehcm]
sudo docker stop fapcorehcm
#remove web container [fapcorehcm]
sudo docker rm fapcorehcm
#remove web image [fapcore/hcm]
sudo docker rmi fapcore/hcm 
#build web image [fapcore/hcm]
sudo docker build -t fapcore/hcm .