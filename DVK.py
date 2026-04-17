import sys
from PyQt5.QtWidgets import (
    QApplication, QWidget, QPushButton, QVBoxLayout,
    QHBoxLayout, QSlider, QLabel, QSpinBox
)
from PyQt5.QtCore import QUrl, QTimer
from PyQt5.QtMultimedia import QMediaPlayer, QMediaContent


class AudioTrack:
    def __init__(self, file_path):
        self.player = QMediaPlayer()
        self.player.setMedia(QMediaContent(QUrl.fromLocalFile(file_path)))
        self.loop = False
        self.delay = 2  # secondi
        self.timer = QTimer()
        self.timer.timeout.connect(self.play)

    def play(self):
        self.player.stop()
        self.player.play()

    def start_loop(self):
        self.loop = True
        self.play()
        self.timer.start(self.delay * 1000)

    def stop_loop(self):
        self.loop = False
        self.timer.stop()
        self.player.stop()

    def set_volume(self, value):
        self.player.setVolume(value)

    def set_delay(self, value):
        self.delay = value
        if self.loop:
            self.timer.start(self.delay * 1000)


class MainWindow(QWidget):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("IZ3NVR/N9SM Contest DVK")

        layout = QVBoxLayout()

        # Insert .wav file path
        self.tracks = [
            AudioTrack("/home/...test.wav"), 
            AudioTrack("/home/...test1.wav"),
            AudioTrack("/home/...test2.wav")
        ]

        for i, track in enumerate(self.tracks):
            row = QHBoxLayout()

            label = QLabel(f"Track {i+1}")
            row.addWidget(label)

            play_btn = QPushButton("Play")
            play_btn.clicked.connect(track.play)
            row.addWidget(play_btn)

            loop_btn = QPushButton("Loop")
            loop_btn.clicked.connect(track.start_loop)
            row.addWidget(loop_btn)

            stop_btn = QPushButton("Stop")
            stop_btn.clicked.connect(track.stop_loop)
            row.addWidget(stop_btn)

            volume = QSlider()
            volume.setMinimum(0)
            volume.setMaximum(100)
            volume.setValue(50)
            volume.valueChanged.connect(track.set_volume)
            row.addWidget(volume)

            delay = QSpinBox()
            delay.setMinimum(1)
            delay.setMaximum(60)
            delay.setValue(2)
            delay.valueChanged.connect(track.set_delay)
            row.addWidget(delay)

            layout.addLayout(row)

        self.setLayout(layout)


if __name__ == "__main__":
    app = QApplication(sys.argv)
    window = MainWindow()
    window.show()
    sys.exit(app.exec_())
